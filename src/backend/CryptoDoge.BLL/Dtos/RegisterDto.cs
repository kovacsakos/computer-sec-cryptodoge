using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Dtos
{
	public class RegisterDto
	{
		public string EmailAddress { get; set; }
		public string Password { get; set; }
		public string UserName { get; set; }
	}
}
