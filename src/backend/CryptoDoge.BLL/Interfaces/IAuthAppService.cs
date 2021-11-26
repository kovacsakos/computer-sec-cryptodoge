using CryptoDoge.BLL.Dtos;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Interfaces
{
    public interface IAuthAppService
	{
		Task<TokenDto> LoginAsync(LoginDto loginDto);
		Task LogoutAsync();
		Task<TokenDto> RegisterAsync(RegisterDto registerDto);
		Task<TokenDto> RenewTokenAsync(string refreshToken);
	}
}
