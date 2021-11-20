using CryptoDoge.DAL;
using CryptoDoge.DAL.Repositories;
using CryptoDoge.Model.DataTransferModels;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Exceptions;
using CryptoDoge.Model.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoDoge.DLL.UnitTests
{
	public class AuthRepositoryTests
	{
		private DbContextOptions<ApplicationDbContext> options;
		private Mock<UserManager<User>> userManager;


		[SetUp]
		public void Setup()
		{
			options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: "CryptoDogeTestDb")
			.Options;

			var _store = new Mock<IUserStore<User>>();
			userManager = new Mock<UserManager<User>>(_store.Object, null, null, null, null, null, null, null, null);

		}

		[Test]
		public async Task GetUserByRefreshTokenAsync_Successful()
		{
			User user;
			using (var context = new ApplicationDbContext(options))
			{
				user = new User
				{
					Id = "id1",
					UserName = "userName",
					NormalizedUserName = "userName",
					Email = "user@mail.com",
					NormalizedEmail = "user@mail.com",
					EmailConfirmed = false,
					RefreshToken = "refreshToken"
				};

				var passwordHasher = new PasswordHasher<User>();

				user.PasswordHash = passwordHasher.HashPassword(user, "Password");

				await context.Users.AddAsync(user);
				await context.SaveChangesAsync();
			}

			using (var context = new ApplicationDbContext(options))
			{
				var refreshToken = "refreshToken";

				var authRepository = new AuthRepository(context,
									userManager.Object);

				var foundUser = await authRepository.GetUserByRefreshTokenAsync(refreshToken);

				Assert.AreEqual(user.Id, foundUser.Id);

				context.Users.Remove(foundUser);
				await context.SaveChangesAsync();
			}
		}

		[Test]
		public async Task GetUserByRefreshTokenAsync_NobodyHasThatRefreshToken()
		{
			User user;
			using (var context = new ApplicationDbContext(options))
			{
				user = new User
				{
					Id = "id2",
					UserName = "userName",
					NormalizedUserName = "userName",
					Email = "user@mail.com",
					NormalizedEmail = "user@mail.com",
					EmailConfirmed = false,
					RefreshToken = "refreshToken"
				};

				var passwordHasher = new PasswordHasher<User>();

				user.PasswordHash = passwordHasher.HashPassword(user, "Password");

				await context.Users.AddAsync(user);
				await context.SaveChangesAsync();
			}

			using (var context = new ApplicationDbContext(options))
			{
				var refreshToken = "nonExistentialRefreshToken";

				var authRepository = new AuthRepository(context,
									userManager.Object);

				try 
				{ 
					var foundUser = await authRepository.GetUserByRefreshTokenAsync(refreshToken); 
				}
				catch(AuthException e)
				{
					Assert.AreEqual(e.Message, "Nobody has that refresh token");

					return;
				}
				finally
				{
					context.Users.Remove(user);
					await context.SaveChangesAsync();
				}

				Assert.Fail();
			}
		}

		[Test]
		public async Task RegisterAsync_Successful()
		{
			User user;
			RegisterData registerData;
			using (var context = new ApplicationDbContext(options))
			{
				user = new User
				{
					Id = "id3",
					UserName = "userName",
					NormalizedUserName = "userName",
					Email = "user@mail.com",
					NormalizedEmail = "user@mail.com",
					EmailConfirmed = false,
					RefreshToken = "refreshToken"
				};

				var passwordHasher = new PasswordHasher<User>();

				user.PasswordHash = passwordHasher.HashPassword(user, "Password");

				await context.Users.AddAsync(user);
				await context.SaveChangesAsync();

				registerData = new RegisterData
				{
					EmailAddress = user.Email,
					UserName = user.UserName,
					Password = "Password"
				};
			}

			userManager.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));

			using (var context = new ApplicationDbContext(options))
			{
				
				var authRepository = new AuthRepository(context,
									userManager.Object);

				try
				{
					await authRepository.RegisterAsync(registerData);
				}
				catch (Exception)
				{
					Assert.Fail();
					return;
				}
				finally
				{
					context.Users.Remove(user);
					await context.SaveChangesAsync();
				}

				Assert.Pass();

			}
		}

		[Test]
		public async Task RegisterAsync_CouldntCreateTheUser()
		{
			User user;
			RegisterData registerData;
			using (var context = new ApplicationDbContext(options))
			{
				user = new User
				{
					Id = "id4",
					UserName = "userName",
					NormalizedUserName = "userName",
					Email = "user@mail.com",
					NormalizedEmail = "user@mail.com",
					EmailConfirmed = false,
					RefreshToken = "refreshToken"
				};

				var passwordHasher = new PasswordHasher<User>();

				user.PasswordHash = passwordHasher.HashPassword(user, "Password");

				await context.Users.AddAsync(user);
				await context.SaveChangesAsync();

				registerData = new RegisterData
				{
					EmailAddress = user.Email,
					UserName = user.UserName,
					Password = "Password"
				};
			}

			userManager.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Failed()));

			using (var context = new ApplicationDbContext(options))
			{

				var authRepository = new AuthRepository(context,
									userManager.Object);

				try
				{
					await authRepository.RegisterAsync(registerData);
				}
				catch (AuthException e)
				{
					Assert.AreEqual(e.Message, "Couldn't create the User");
					return;
				}
				finally
				{
					context.Users.Remove(user);
					await context.SaveChangesAsync();
				}

				Assert.Fail();
			}
		}

		[Test]
		public async Task RemoveRefreshTokenAsync_Successful()
		{
			User user;
			RegisterData registerData;
			using (var context = new ApplicationDbContext(options))
			{
				user = new User
				{
					Id = "id5",
					UserName = "userName",
					NormalizedUserName = "userName",
					Email = "user@mail.com",
					NormalizedEmail = "user@mail.com",
					EmailConfirmed = false,
					RefreshToken = "refreshToken"
				};

				var passwordHasher = new PasswordHasher<User>();

				user.PasswordHash = passwordHasher.HashPassword(user, "Password");

				await context.Users.AddAsync(user);
				await context.SaveChangesAsync();

				registerData = new RegisterData
				{
					EmailAddress = user.Email,
					UserName = user.UserName,
					Password = "Password"
				};
			}

			userManager.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Failed()));

			using (var context = new ApplicationDbContext(options))
			{

				var authRepository = new AuthRepository(context,
									userManager.Object);

				await authRepository.RemoveRefreshTokenAsync(user.Id);
				var foundUser = await context.Users.SingleOrDefaultAsync(r => r.Id == user.Id);

				if(foundUser == null || foundUser.RefreshToken != string.Empty)
					Assert.Fail();

				if(foundUser.RefreshToken == string.Empty)
					Assert.Pass();


				context.Users.Remove(user);
				await context.SaveChangesAsync();
			}
		}

		[Test]
		public async Task SaveRefreshTokenAsync_Successful()
		{
			User user;
			RegisterData registerData;
			using (var context = new ApplicationDbContext(options))
			{
				user = new User
				{
					Id = "id6",
					UserName = "userName",
					NormalizedUserName = "userName",
					Email = "user@mail.com",
					NormalizedEmail = "user@mail.com",
					EmailConfirmed = false,
					RefreshToken = "refreshToken"
				};

				var passwordHasher = new PasswordHasher<User>();

				user.PasswordHash = passwordHasher.HashPassword(user, "Password");

				await context.Users.AddAsync(user);
				await context.SaveChangesAsync();

				registerData = new RegisterData
				{
					EmailAddress = user.Email,
					UserName = user.UserName,
					Password = "Password"
				};
			}

			userManager.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Failed()));

			using (var context = new ApplicationDbContext(options))
			{
				var newRefreshToken = "newRefreshToken";
				var authRepository = new AuthRepository(context,
									userManager.Object);

				await authRepository.SaveRefreshTokenAsync(user, newRefreshToken);
				var foundUser = await context.Users.SingleOrDefaultAsync(r => r.Id == user.Id);

				if (foundUser == null || foundUser.RefreshToken == string.Empty)
					Assert.Fail();

				if (foundUser.RefreshToken == newRefreshToken)
					Assert.Pass();


				context.Users.Remove(foundUser);
				await context.SaveChangesAsync();
			}
		}

		[Test]
		public async Task SaveRefreshTokenAsync_Unsuccessful()
		{
			User user;
			RegisterData registerData;
			using (var context = new ApplicationDbContext(options))
			{
				user = new User
				{
					Id = "id7",
					UserName = "userName",
					NormalizedUserName = "userName",
					Email = "user@mail.com",
					NormalizedEmail = "user@mail.com",
					EmailConfirmed = false,
					RefreshToken = "refreshToken"
				};

				var passwordHasher = new PasswordHasher<User>();

				user.PasswordHash = passwordHasher.HashPassword(user, "Password");

				await context.Users.AddAsync(user);
				await context.SaveChangesAsync();

				registerData = new RegisterData
				{
					EmailAddress = user.Email,
					UserName = user.UserName,
					Password = "Password"
				};
			}

			userManager.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Failed()));

			using (var context = new ApplicationDbContext(options))
			{
				var nonExistentUser = new User { Id = "nonExistentId" };
				var newRefreshToken = "newRefreshToken";
				var authRepository = new AuthRepository(context,
									userManager.Object);

				try
				{
					await authRepository.SaveRefreshTokenAsync(nonExistentUser, newRefreshToken);
				}
				catch(NotFoundException e)
				{
					Assert.AreEqual(e.Message, $"User was not found with ID: {nonExistentUser.Id}");
					return;
				}
				finally
				{
					context.Users.Remove(user);
					await context.SaveChangesAsync();
				}

				Assert.Fail();
			}
		}
	}
}