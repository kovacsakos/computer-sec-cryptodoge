using CryptoDoge.Model.DataTransferModels;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Exceptions;
using CryptoDoge.Model.Interfaces;
using CryptoDoge.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CryptoDoge.DAL.Repositories
{
    public class AuthRepository : IAuthRepository
	{
		private readonly ApplicationDbContext dbContext;
		private readonly UserManager<User> userManager;
        private readonly ILogger<AuthRepository> logger;

        public AuthRepository(ApplicationDbContext dbContext,
								UserManager<User> userManager,
								ILogger<AuthRepository> logger)
		{
			this.dbContext = dbContext;
			this.userManager = userManager;
            this.logger = logger;
        }

		public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
		{
			using var loggerScope = new LoggerScope(logger);
			var userToFind = await dbContext.Users.SingleOrDefaultAsync(user => user.RefreshToken == refreshToken);

			if (userToFind == null)
			{
				var ex =  new AuthException("Nobody has that refresh token");
				logger.LogWarning(ex.Message);
				throw ex;
			}
			return userToFind;
		}

		public async Task RegisterAsync(RegisterData registerData)
		{
			using var loggerScope = new LoggerScope(logger);
			var user = new User
			{
				UserName = registerData.UserName,
				Email = registerData.EmailAddress,
				NormalizedUserName = registerData.UserName,
				NormalizedEmail = registerData.EmailAddress,
			};

			var result = await userManager.CreateAsync(user, registerData.Password);

			if (!result.Succeeded)
			{
				var ex = new AuthException("Couldn't create the User");
				logger.LogWarning(ex.Message);
				throw ex;
			}
		}

		public async Task RemoveRefreshTokenAsync(string userId)
		{
			await SetRefreshToken(userId, string.Empty);
		}

		public async Task SaveRefreshTokenAsync(User user, string refreshToken)
		{
			await SetRefreshToken(user.Id, refreshToken);
		}

		private async Task SetRefreshToken(string userId, string refreshToken)
		{
			var userToFind = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);

			if (userToFind == null)
			{
				var ex = new NotFoundException($"User was not found with ID: {userId}");
				logger.LogWarning(ex.Message);
				throw ex;
			}

			userToFind.RefreshToken = refreshToken;
			await dbContext.SaveChangesAsync();
		}
	}
}
