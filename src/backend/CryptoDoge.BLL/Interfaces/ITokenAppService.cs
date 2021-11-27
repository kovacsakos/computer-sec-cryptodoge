using CryptoDoge.Model.Entities;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Interfaces
{
    public interface ITokenAppService
	{
		Task<string> CreateAccessToken(User user);
		Task<string> CreateRefreshTokenAsync(User user);
		Task RemoveRefreshToken(string userId);
	}
}
