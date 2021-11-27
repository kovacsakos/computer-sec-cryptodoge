using CryptoDoge.Model.DataTransferModels;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Exceptions;
using CryptoDoge.Model.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CryptoDoge.DAL.Repositories
{
    public class AuthRepository : IAuthRepository
	{
		private readonly ApplicationDbContext dbContext;
		private readonly UserManager<User> userManager;

		public AuthRepository(ApplicationDbContext dbContext,
								UserManager<User> userManager)
		{
			this.dbContext = dbContext;
			this.userManager = userManager;
		}

		public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
		{
			var userToFind = await dbContext.Users.SingleOrDefaultAsync(user => user.RefreshToken == refreshToken);

			if (userToFind == null)
			{
				throw new AuthException("Nobody has that refresh token");
			}
			return userToFind;
		}

		public async Task RegisterAsync(RegisterData registerData)
		{
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
				throw new AuthException("Couldn't create the User");
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
				throw new NotFoundException($"User was not found with ID: {userId}");
			}

			userToFind.RefreshToken = refreshToken;
			await dbContext.SaveChangesAsync();
		}
	}
}
