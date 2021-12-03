using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.ParserService
{
    [Serializable]
    public class ParserException : Exception
    {
        public ParserException() : base() { }

        public ParserException(string message) : base(message) { }

        public ParserException(string message, Exception innerException) : base(message, innerException) { }
    }
}
