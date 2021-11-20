using CryptoDoge.Model.DataTransferModels;
using CryptoDoge.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
