using AutoMapper;
using CryptoDoge.BLL.Dtos;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.BLL.Services;
using CryptoDoge.Model.DataTransferModels;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Exceptions;
using CryptoDoge.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.UnitTests
{
	public class AuthAppServiceTests
	{
		private IAuthAppService authAppservice;
		private Mock<UserManager<User>> userManager;
		private Mock<SignInManager<User>> signInManager;
		private Mock<ITokenAppService> tokenService;
		private Mock<IAuthRepository> authRepository;
		private Mock<IMapper> mapper;
		private Mock<IIdentityService> identityService;

		[SetUp]
		public void Setup()
		{

			var _store = new Mock<IUserStore<User>>();
			userManager = new Mock<UserManager<User>>(_store.Object, null, null, null, null, null, null, null, null);

			var _contextAccessor = new Mock<IHttpContextAccessor>();
			var _userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
			signInManager = new Mock<SignInManager<User>>(userManager.Object,
						   _contextAccessor.Object, _userPrincipalFactory.Object, null, null, null);

			tokenService = new Mock<ITokenAppService>();
			authRepository = new Mock<IAuthRepository>();
			mapper = new Mock<IMapper>();
			identityService = new Mock<IIdentityService>();

			authAppservice = new AuthAppService(userManager.Object,
								signInManager.Object,
								tokenService.Object,
								authRepository.Object,
								mapper.Object,
								identityService.Object);
		}

		[Test]
		public async Task LoginAsync_Sucessful()
		{
			var user = new User();

			var signInResult = SignInResult.Success;

			var loginDto = new LoginDto();

			var accessToken = "accessToken";
			var refreshToken = "refreshToken";

			userManager.Setup(r => r.FindByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

			signInManager.Setup(r => r.PasswordSignInAsync(user, It.IsAny<string>(), true, false)).Returns(Task.FromResult(signInResult));

			tokenService.Setup(r => r.CreateAccessToken(user)).Returns(Task.FromResult(accessToken));
			tokenService.Setup(r => r.CreateRefreshTokenAsync(user)).Returns(Task.FromResult(refreshToken));

			var tokenDto = await authAppservice.LoginAsync(loginDto);

			Assert.AreEqual(tokenDto.AccessToken, accessToken);
			Assert.AreEqual(tokenDto.RefreshToken, refreshToken);
		}

		[Test]
		public void LoginAsync_WrongUsernameOrPassword()
		{
			var user = new User();

			var signInResult = SignInResult.Success;

			var loginDto = new LoginDto();

			var accessToken = "accessToken";
			var refreshToken = "refreshToken";

			userManager.Setup(r => r.FindByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));

			signInManager.Setup(r => r.PasswordSignInAsync(user, It.IsAny<string>(), true, false)).Returns(Task.FromResult(signInResult));

			tokenService.Setup(r => r.CreateAccessToken(user)).Returns(Task.FromResult(accessToken));
			tokenService.Setup(r => r.CreateRefreshTokenAsync(user)).Returns(Task.FromResult(refreshToken));

			var exception = Assert.ThrowsAsync<AuthException>(() => authAppservice.LoginAsync(loginDto));
			Assert.AreEqual(exception.Message, "Wrong username or password");
		}

		[Test]
		public void LoginAsync_UnsuccessfulLogin()
		{
			var user = new User();

			var signInResult = SignInResult.Failed;

			var loginDto = new LoginDto();

			var accessToken = "accessToken";
			var refreshToken = "refreshToken";

			userManager.Setup(r => r.FindByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

			signInManager.Setup(r => r.PasswordSignInAsync(user, It.IsAny<string>(), true, false)).Returns(Task.FromResult(signInResult));

			tokenService.Setup(r => r.CreateAccessToken(user)).Returns(Task.FromResult(accessToken));
			tokenService.Setup(r => r.CreateRefreshTokenAsync(user)).Returns(Task.FromResult(refreshToken));

			var exception = Assert.ThrowsAsync<AuthException>(() => authAppservice.LoginAsync(loginDto));
			Assert.AreEqual(exception.Message, "Login was not successful");
		}

		[Test]
		public void LogoutAsync_Successful()
		{
			var userId = "userId";

			identityService.Setup(r => r.GetCurrentUserIdAsync()).Returns(Task.FromResult(userId));

			Assert.DoesNotThrowAsync(() => authAppservice.LogoutAsync());

			identityService.Verify(mock => mock.GetCurrentUserIdAsync(), Times.Once());
			signInManager.Verify(mock => mock.SignOutAsync(), Times.Once());
			authRepository.Verify(mock => mock.RemoveRefreshTokenAsync(userId), Times.Once());
		}

		[Test]
		public async Task RegisterAsync_Successful()
		{
			var registerDto = new RegisterDto();
			var userId = "userId";
			var createdUser = new User();
			var registerData = new RegisterData();
			var signInResult = SignInResult.Success;
			var accessToken = "accessToken";
			var refreshToken = "refreshToken";

			identityService.Setup(r => r.GetCurrentUserIdAsync()).Returns(Task.FromResult(userId));

			userManager.SetupSequence(r => r.FindByEmailAsync(It.IsAny<string>()))
				.Returns(Task.FromResult<User>(null))
				.Returns(Task.FromResult(createdUser));

			mapper.Setup(r => r.Map<RegisterData>(It.IsAny<RegisterDto>())).Returns(registerData);

			signInManager.Setup(r => r.PasswordSignInAsync(createdUser, It.IsAny<string>(), true, false)).Returns(Task.FromResult(signInResult));

			tokenService.Setup(r => r.CreateAccessToken(createdUser)).Returns(Task.FromResult(accessToken));
			tokenService.Setup(r => r.CreateRefreshTokenAsync(createdUser)).Returns(Task.FromResult(refreshToken));

			var tokenDto = await authAppservice.RegisterAsync(registerDto);

			Assert.AreEqual(tokenDto.AccessToken, accessToken);
			Assert.AreEqual(tokenDto.RefreshToken, refreshToken);
		}

		[Test]
		public async Task RegisterAsync_ExistingLogin()
		{
			var registerDto = new RegisterDto();
			var userId = "userId";
			var createdUser = new User();
			var registerData = new RegisterData();
			var signInResult = SignInResult.Success;
			var accessToken = "accessToken";
			var refreshToken = "refreshToken";

			identityService.Setup(r => r.GetCurrentUserIdAsync()).Returns(Task.FromResult(userId));

			userManager.SetupSequence(r => r.FindByEmailAsync(It.IsAny<string>()))
				.Returns(Task.FromResult<User>(null))
				.Returns(Task.FromResult(createdUser));

			mapper.Setup(r => r.Map<RegisterData>(It.IsAny<RegisterDto>())).Returns(registerData);

			signInManager.Setup(r => r.PasswordSignInAsync(createdUser, It.IsAny<string>(), true, false)).Returns(Task.FromResult(signInResult));

			tokenService.Setup(r => r.CreateAccessToken(createdUser)).Returns(Task.FromResult(accessToken));
			tokenService.Setup(r => r.CreateRefreshTokenAsync(createdUser)).Returns(Task.FromResult(refreshToken));

			var tokenDto = await authAppservice.RegisterAsync(registerDto);

			signInManager.Verify(mock => mock.SignOutAsync(), Times.Once());
			authRepository.Verify(mock => mock.RemoveRefreshTokenAsync(It.IsAny<string>()), Times.Once());
		}

		[Test]
		public void RegisterAsync_EmailTaken()
		{
			var registerDto = new RegisterDto();
			var userId = "userId";
			var createdUser = new User();
			var registerData = new RegisterData();
			var signInResult = SignInResult.Success;
			var accessToken = "accessToken";
			var refreshToken = "refreshToken";

			identityService.Setup(r => r.GetCurrentUserIdAsync()).Returns(Task.FromResult(userId));

			userManager.SetupSequence(r => r.FindByEmailAsync(It.IsAny<string>()))
				.Returns(Task.FromResult(createdUser))
				.Returns(Task.FromResult(createdUser));

			mapper.Setup(r => r.Map<RegisterData>(It.IsAny<RegisterDto>())).Returns(registerData);

			signInManager.Setup(r => r.PasswordSignInAsync(createdUser, It.IsAny<string>(), true, false)).Returns(Task.FromResult(signInResult));

			tokenService.Setup(r => r.CreateAccessToken(createdUser)).Returns(Task.FromResult(accessToken));
			tokenService.Setup(r => r.CreateRefreshTokenAsync(createdUser)).Returns(Task.FromResult(refreshToken));

			var exception = Assert.ThrowsAsync<AuthException>(() => authAppservice.RegisterAsync(registerDto));
			Assert.AreEqual(exception.Message, "Email taken");
		}

		[Test]
		public void RegisterAsync_CouldntCreateUser()
		{
			var registerDto = new RegisterDto();
			var userId = "userId";
			var createdUser = new User();
			var registerData = new RegisterData();
			var signInResult = SignInResult.Success;
			var accessToken = "accessToken";
			var refreshToken = "refreshToken";

			identityService.Setup(r => r.GetCurrentUserIdAsync()).Returns(Task.FromResult(userId));

			userManager.SetupSequence(r => r.FindByEmailAsync(It.IsAny<string>()))
				.Returns(Task.FromResult<User>(null))
				.Returns(Task.FromResult<User>(null));

			mapper.Setup(r => r.Map<RegisterData>(It.IsAny<RegisterDto>())).Returns(registerData);

			signInManager.Setup(r => r.PasswordSignInAsync(createdUser, It.IsAny<string>(), true, false)).Returns(Task.FromResult(signInResult));

			tokenService.Setup(r => r.CreateAccessToken(createdUser)).Returns(Task.FromResult(accessToken));
			tokenService.Setup(r => r.CreateRefreshTokenAsync(createdUser)).Returns(Task.FromResult(refreshToken));

			var exception = Assert.ThrowsAsync<AuthException>(() => authAppservice.RegisterAsync(registerDto));
			Assert.AreEqual(exception.Message, "Couldn't create user. Try with different values");
		}

		[Test]
		public void RegisterAsync_CantLogin()
		{
			var registerDto = new RegisterDto();
			var userId = "userId";
			var createdUser = new User();
			var registerData = new RegisterData();
			var signInResult = SignInResult.Failed;
			var accessToken = "accessToken";
			var refreshToken = "refreshToken";

			identityService.Setup(r => r.GetCurrentUserIdAsync()).Returns(Task.FromResult(userId));

			userManager.SetupSequence(r => r.FindByEmailAsync(It.IsAny<string>()))
				.Returns(Task.FromResult<User>(null))
				.Returns(Task.FromResult(createdUser));

			mapper.Setup(r => r.Map<RegisterData>(It.IsAny<RegisterDto>())).Returns(registerData);

			signInManager.Setup(r => r.PasswordSignInAsync(createdUser, It.IsAny<string>(), true, false)).Returns(Task.FromResult(signInResult));

			tokenService.Setup(r => r.CreateAccessToken(createdUser)).Returns(Task.FromResult(accessToken));
			tokenService.Setup(r => r.CreateRefreshTokenAsync(createdUser)).Returns(Task.FromResult(refreshToken));

			var exception = Assert.ThrowsAsync<AuthException>(() => authAppservice.RegisterAsync(registerDto));
			Assert.AreEqual(exception.Message, "Can't login, try again");
		}

		[Test]
		public async Task RenewTokenAsync_Successful()
		{
			var user = new User();
			var accessToken = "accessToken";
			var refreshToken = "refreshToken";

			authRepository.Setup(r => r.GetUserByRefreshTokenAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

			tokenService.Setup(r => r.CreateAccessToken(user)).Returns(Task.FromResult(accessToken));
			tokenService.Setup(r => r.CreateRefreshTokenAsync(user)).Returns(Task.FromResult(refreshToken));

			var tokenDto = await authAppservice.RenewTokenAsync(It.IsAny<string>());

			Assert.AreEqual(tokenDto.AccessToken, accessToken);
			Assert.AreEqual(tokenDto.RefreshToken, refreshToken);
		}

		[Test]
		public void RenewTokenAsync_NoUserWithThatRefreshToken()
		{
			var user = new User();
			var accessToken = "accessToken";
			var refreshToken = "refreshToken";

			authRepository.Setup(r => r.GetUserByRefreshTokenAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));

			tokenService.Setup(r => r.CreateAccessToken(user)).Returns(Task.FromResult(accessToken));
			tokenService.Setup(r => r.CreateRefreshTokenAsync(user)).Returns(Task.FromResult(refreshToken));

			var exception = Assert.ThrowsAsync<AuthException>(() => authAppservice.RenewTokenAsync(It.IsAny<string>()));
			Assert.AreEqual(exception.Message, "There is no user with that specific refresh token");
		}
	}
}