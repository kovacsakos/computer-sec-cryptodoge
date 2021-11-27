using System;

namespace CryptoDoge.Model.Exceptions
{
    public class NotFoundException : Exception
	{
		public int? ErrorCode { get; set; }

		public NotFoundException(string message, int? errorCode = null) : base(message)
		{
			ErrorCode = errorCode;
		}
	}
}
