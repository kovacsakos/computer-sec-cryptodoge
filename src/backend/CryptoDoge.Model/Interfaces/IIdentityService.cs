using CryptoDoge.Model.Entities;
using System.Threading.Tasks;

namespace CryptoDoge.Model.Interfaces
{
    public interface IIdentityService
	{
		Task<string> GetCurrentUserIdAsync();
		Task<User> GetCurrentUserAsync();
	}
}
