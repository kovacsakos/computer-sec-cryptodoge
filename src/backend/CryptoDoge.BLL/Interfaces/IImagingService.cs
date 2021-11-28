﻿using CryptoDoge.BLL.Dtos;
using CryptoDoge.Model.Entities;
using CryptoDoge.ParserService;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Interfaces
{
    public interface IImagingService
    {
        Task<CaffDto> SaveCaffImagesAsync(ParsedCaff parsedCaff, User user);
        Task<CaffDto> SaveCaffAsync(ParsedCaff parsedCaff, User user, IFormFile rawfile);
        Task<IEnumerable<CaffDto>> GetCaffsAsync();
        Task<CaffDto> GetCaffByIdAsync(string caffId);
        Task<byte[]> GetRawCaffByIdAsync(string caffId);
        Task DeleteCaffImagesAsync(string caffId);
        Task<string> AddCaffCommentAsync(string caffId, string comment, User user);
        Task<CaffComment> GetCaffCommentByIdAsync(string id);
        Task DeleteCaffCommentAsync(string caffCommentId);
        Task UpdateCommentOnCaffAsync(string caffCommentId, string comment);
        Task<IEnumerable<CaffDto>> SearchCaffsByCaption(string query);
        Task<IEnumerable<CaffDto>> SearchCaffsByTags(List<string> queryTags);
    }
}