using CryptoDoge.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
