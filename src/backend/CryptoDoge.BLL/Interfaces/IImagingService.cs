using CryptoDoge.ParserService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Interfaces
{
    public interface IImagingService
    {
        Task<IEnumerable<string>> SaveCaffImagesAsync(ParsedCaff parsedCaff);
    }
}