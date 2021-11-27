using System;
using System.Runtime.Serialization;

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
