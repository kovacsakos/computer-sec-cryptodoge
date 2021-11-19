using CryptoDoge.Shared.Models;
using System.IO;

namespace CryptoDoge.ParserService
{
    public interface IParserService
    {
        Caff GetCaffFromFile(string filePath);
        Caff GetCaffFromMemoryStream(MemoryStream caffMemoryStream);
        Caff GetCaffFromByteArray(byte[] buffer);
    }
}