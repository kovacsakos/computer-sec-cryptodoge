using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.Shared
{
    public class CryptoRandomGenerator : ICryptoRandomGenerator
    {
        public string GenerateRandomString(int length = 32)
        {
            byte[] data = new byte[length / 2];
            RandomNumberGenerator.Fill(data);
            return BitConverter.ToString(data).Replace("-", "").ToLower();
        }

    }
}
