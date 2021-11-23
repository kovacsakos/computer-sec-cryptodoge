using CryptoDoge.BLL.Interfaces;
using CryptoDoge.BLL.Services;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Interfaces;
using CryptoDoge.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.UnitTests
{

	public class TokenAppServiceTests
	{
		private ITokenAppService tokenAppService;

		private Mock<IConfiguration> configuration;
		private Mock<IAuthRepository> authRepository;
		private Mock<UserManager<User>> userManager;
		private Mock<ICryptoRandomGenerator> randomGenerator;

		[SetUp]
		public void Setup()
		{
			configuration = new Mock<IConfiguration>();
			authRepository = new Mock<IAuthRepository>();

			var _store = new Mock<IUserStore<User>>();
			userManager = new Mock<UserManager<User>>(_store.Object, null, null, null, null, null, null, null, null);

			randomGenerator = new Mock<ICryptoRandomGenerator>();

			tokenAppService = new TokenAppService(configuration.Object,
													authRepository.Object,
													userManager.Object,
													randomGenerator.Object);
		}

		[Test]
		public async Task CreateAccessToken_Successful()
		{
			var user = new User()
			{
				UserName = "userName",
				Email = "email",
				Id = "id"
			};
			var userRoles = new List<string>() as IList<string>;

			var jwtKey = "9dda9a1c-2548-42d8-8614-66284f5a7b74";
			var jwtDefaultIssuer = "0a9136a8-5b1f-4314-85b4-c260cf83eda4";
			var JwtExpirationMinutes = "120";

			userManager.Setup(r => r.GetRolesAsync(user)).Returns(Task.FromResult(userRoles));

			configuration.SetupSequence(r => r.GetSection(It.IsAny<string>())[It.IsAny<string>()])
				.Returns(jwtKey)
				.Returns(jwtDefaultIssuer)
				.Returns(JwtExpirationMinutes);

			userManager.Setup(r => r.IsInRoleAsync(user, "Admin")).Returns(Task.FromResult(true));

			var token = await tokenAppService.CreateAccessToken(user);

			configuration.Verify(mock => mock.GetSection("Authentication")[It.IsAny<string>()], Times.Exactly(3));
		}

		[Test]
		public async Task CreateRefreshToken_Successful()
		{
			var user = new User();
			var refreshToken = "refreshToken";

			randomGenerator.Setup(r => r.GenerateRandomString(It.IsAny<int>())).Returns(refreshToken);

			var token = await tokenAppService.CreateRefreshTokenAsync(user);

			authRepository.Verify(mock => mock.SaveRefreshTokenAsync(user, refreshToken), Times.Once());

			Assert.AreEqual(token, refreshToken);
		}

		[Test]
		public async Task RemoveRefreshToken_Successful()
		{
			var userId = "userId";

			await tokenAppService.RemoveRefreshToken(userId);

			authRepository.Verify(mock => mock.RemoveRefreshTokenAsync(userId), Times.Once());
		}
	}
}
