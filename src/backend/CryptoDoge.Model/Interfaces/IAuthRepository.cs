using CryptoDoge.Model.DataTransferModels;
using CryptoDoge.Model.Entities;
using System.Threading.Tasks;

namespace CryptoDoge.Model.Interfaces
{
    public interface IAuthRepository
    {
        Task RegisterAsync(RegisterData registerData);
        Task<User> GetUserByRefreshTokenAsync(string refreshToken);
        Task RemoveRefreshTokenAsync(string userId);
        Task SaveRefreshTokenAsync(User user, string refreshToken);
    }
}
