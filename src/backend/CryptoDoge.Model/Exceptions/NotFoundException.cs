using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
