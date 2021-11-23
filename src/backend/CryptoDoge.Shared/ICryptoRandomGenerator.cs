using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.Shared
{
    public interface ICryptoRandomGenerator
    {
        public string GenerateRandomString(int length = 32);
    }
}
