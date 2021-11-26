using CryptoDoge.BLL.Dtos;
using CryptoDoge.Model.Entities;
using CryptoDoge.ParserService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Interfaces
{
    public interface IImagingService
    {
        Task<CaffDto> SaveCaffImagesAsync(ParsedCaff parsedCaff, User user);
        Task<IEnumerable<CaffDto>> GetCaffsAsync();
        Task<CaffDto> GetCaffByIdAsync(string id);
        Task DeleteCaffImagesAsync(string id);
        Task AddCaffCommentAsync(CaffCommentDto caffCommentDtod);
        Task DeleteCaffCommentAsync(string id);
        Task UpdateCommentOnCaffAsync(string id, CaffCommentUpdateDto caffCommentUpdateDto);
        Task<IEnumerable<CaffDto>> SearchCaffsByCaption(string query);
        Task<IEnumerable<CaffDto>> SearchCaffsByTags(List<string> queryTags);
    }
}