namespace CryptoDoge.Shared
{
    public interface ICryptoRandomGenerator
    {
        public string GenerateRandomString(int length = 32);
    }
}
