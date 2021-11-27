using Microsoft.AspNetCore.Identity;

namespace CryptoDoge.Model.Entities
{
    public class User: IdentityUser
	{
		public string RefreshToken { get; set; }

	}
}
