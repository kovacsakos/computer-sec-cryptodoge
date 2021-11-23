using AutoMapper;
using CryptoDoge.BLL.Dtos;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.Model.DataTransferModels;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Exceptions;
using CryptoDoge.Model.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public AuthAppService(UserManager<User> userManager,
                                SignInManager<User> signInManager,
                                ITokenAppService tokenService,
                                IAuthRepository userRepository,
                                IMapper mapper,
                                IIdentityService identityService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.identityService = identityService;
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.EmailAddress);

            if (user == null)
            {
                throw new AuthException("Wrong username or password");
            }

            var result = await signInManager.PasswordSignInAsync(user, loginDto.Password, true, false);
            if (!result.Succeeded)
            {
                throw new AuthException("Login was not successful");
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
            var userId = await identityService.GetCurrentUserIdAsync();
            if (!string.IsNullOrEmpty(userId))
            {
                await signInManager.SignOutAsync();
                await userRepository.RemoveRefreshTokenAsync(userId);
            }

            var storedUser = await userManager.FindByEmailAsync(registerDto.EmailAddress);

            if (storedUser != null)
            {
                throw new AuthException("Email taken");
            }

            var registerData = mapper.Map<RegisterData>(registerDto);
            await userRepository.RegisterAsync(registerData);

            var createdUser = await userManager.FindByEmailAsync(registerDto.EmailAddress);

            if (createdUser == null)
            {
                throw new AuthException("Couldn't create user. Try with different values");
            }

            await userManager.AddToRoleAsync(createdUser, "Default");

            var result = await signInManager.PasswordSignInAsync(createdUser, registerDto.Password, true, false);
            if (!result.Succeeded)
            {
                throw new AuthException("Can't login, try again");
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
                throw new AuthException("There is no user with that specific refresh token");
            }
            return new TokenDto
            {
                AccessToken = await tokenService.CreateAccessToken(user),
                RefreshToken = await tokenService.CreateRefreshTokenAsync(user)
            };
        }
    }
}
