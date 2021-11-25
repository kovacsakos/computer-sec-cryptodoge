using System.IO;

namespace CryptoDoge.ParserService
{
    public interface IParserService
    {
        ParsedCaff GetCaffFromFile(string filePath);
        ParsedCaff GetCaffFromMemoryStream(MemoryStream caffMemoryStream);
        ParsedCaff GetCaffFromByteArray(byte[] buffer);
    }
}