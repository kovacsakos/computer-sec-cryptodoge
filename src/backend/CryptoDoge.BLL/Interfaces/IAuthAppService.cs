using CryptoDoge.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Interfaces
{
	public interface IAuthAppService
	{
		Task<TokenDto> LoginAsync(LoginDto authDto);
		Task LogoutAsync();
		Task<TokenDto> RegisterAsync(RegisterDto authDto);
		Task<TokenDto> RenewTokenAsync(string refreshToken);
	}
}
