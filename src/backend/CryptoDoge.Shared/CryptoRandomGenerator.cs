using System;
using System.Security.Cryptography;

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
