﻿using CryptoDoge.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoDoge.Model.Interfaces
{
    public interface ICaffRepository
    {
        Task AddNewCaffAsync(Caff caff);
        Task<IEnumerable<Caff>> GetCaffsAsync();
        Task<Caff> GetCaffByIdAsync(string caffId);
        Task DeleteCaffAsync(Caff caff);
        Task AddCaffCommentAsync(CaffComment caffComment);
        Task<CaffComment> GetCaffCommentByIdAsync(string id);
        Task DeleteCaffCommentAsync(CaffComment caffComment);
        Task UpdateCaffCommentAsync(CaffComment caffComment);
        Task<IEnumerable<Caff>> SearchCaffsByCaption(string query);
        Task<IEnumerable<Caff>> SearchCaffsByTags(List<string> queryTags);
    }
}
