using CryptoDoge.BLL.Dtos;
using CryptoDoge.Model.Entities;
using CryptoDoge.ParserService;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Interfaces
{
    public interface IImagingService
    {
        Task<CaffDto> SaveCaffImagesAsync(ParsedCaff parsedCaff, User currentUser);
        public Task<CaffDto> GetCaffByIdAsync(string id);
        Task DeleteCaffImagesAsync(string id, User user);
        Task CommentOnCaff(CaffCommentDto caffCommentDto, User user);
    }
}