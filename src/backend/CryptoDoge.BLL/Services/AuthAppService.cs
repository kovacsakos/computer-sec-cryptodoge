using AutoMapper;
using CryptoDoge.BLL.Dtos;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.Model.DataTransferModels;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Exceptions;
using CryptoDoge.Model.Interfaces;
using CryptoDoge.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Services
{
    public class AuthAppService : IAuthAppService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ITokenAppService tokenService;
        private readonly IAuthRepository userRepository;
        private readonly IMapper mapper;
        private readonly IIdentityService identityService;
        private readonly ILogger<AuthAppService> logger;

        public AuthAppService(UserManager<User> userManager,
                                SignInManager<User> signInManager,
                                ITokenAppService tokenService,
                                IAuthRepository userRepository,
                                IMapper mapper,
                                IIdentityService identityService,
                                ILogger<AuthAppService> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.identityService = identityService;
            this.logger = logger;
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            using var loggerScope = new LoggerScope(logger);
            var user = await userManager.FindByEmailAsync(loginDto.EmailAddress);

            if (user == null)
            {
                var ex = new AuthException("Wrong username or password");
                logger.LogWarning(ex.Message);
                throw ex;
            }

            var result = await signInManager.PasswordSignInAsync(user, loginDto.Password, true, true);
            if (!result.Succeeded)
            {
                var ex = new AuthException("Login was not successful");
                logger.LogWarning(ex.Message);
                throw ex;
            }

            return new TokenDto
            {
                AccessToken = await tokenService.CreateAccessToken(user),
                RefreshToken = await tokenService.CreateRefreshTokenAsync(user)
            };
        }

        public async Task LogoutAsync()
        {
            var userId = await identityService.GetCurrentUserIdAsync();
            await signInManager.SignOutAsync();
            await userRepository.RemoveRefreshTokenAsync(userId);
        }

        public async Task<TokenDto> RegisterAsync(RegisterDto registerDto)
        {
            using var loggerScope = new LoggerScope(logger);
            var userId = await identityService.GetCurrentUserIdAsync();
            if (!string.IsNullOrEmpty(userId))
            {
                await signInManager.SignOutAsync();
                await userRepository.RemoveRefreshTokenAsync(userId);
            }

            var storedUser = await userManager.FindByEmailAsync(registerDto.EmailAddress);

            if (storedUser != null)
            {
                var ex = new AuthException("Email taken");
                logger.LogWarning(ex.Message);
                throw ex;

            }

            var registerData = mapper.Map<RegisterData>(registerDto);
            await userRepository.RegisterAsync(registerData);

            var createdUser = await userManager.FindByEmailAsync(registerDto.EmailAddress);

            if (createdUser == null)
            {
                var ex = new AuthException("Couldn't create user. Try with different values");
                logger.LogWarning(ex.Message);
                throw ex;
            }

            await userManager.AddToRoleAsync(createdUser, "USER");

            var result = await signInManager.PasswordSignInAsync(createdUser, registerDto.Password, true, false);
            if (!result.Succeeded)
            {
                var ex = new AuthException("Can't login, try again");
                logger.LogWarning(ex.Message);
                throw ex;
            }

            return new TokenDto
            {
                AccessToken = await tokenService.CreateAccessToken(createdUser),
                RefreshToken = await tokenService.CreateRefreshTokenAsync(createdUser)
            };
        }

        public async Task<TokenDto> RenewTokenAsync(string refreshToken)
        {
            var user = await userRepository.GetUserByRefreshTokenAsync(refreshToken);

            if (user == null)
            {
                var ex = new AuthException("There is no user with that specific refresh token");
                logger.LogWarning(ex.Message);
                throw ex;

            }
            return new TokenDto
            {
                AccessToken = await tokenService.CreateAccessToken(user),
                RefreshToken = await tokenService.CreateRefreshTokenAsync(user)
            };
        }
    }
}
