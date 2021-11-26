using CryptoDoge.BLL.Dtos;
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
