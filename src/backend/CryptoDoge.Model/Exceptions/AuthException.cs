using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.Model.Exceptions
{
	public class AuthException : Exception
	{
		public int? ErrorCode { get; set; }

		public AuthException(string message, int? errorCode = null) : base(message)
		{
			ErrorCode = errorCode;
		}
	}
}
