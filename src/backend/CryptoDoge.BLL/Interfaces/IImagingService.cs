using CryptoDoge.BLL.Dtos;
using CryptoDoge.ParserService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Interfaces
{
    public interface IImagingService
    {
        Task<CaffDto> SaveCaffImagesAsync(ParsedCaff parsedCaff);
        public Task<CaffDto> GetCaffByIdAsync(string id);
        Task DeleteCaffImagesAsync(string id);
    }
}