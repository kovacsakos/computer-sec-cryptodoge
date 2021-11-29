using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoDoge.DAL.Repositories
{
    public class CaffRepository : ICaffRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CaffRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddNewCaffAsync(Caff caff)
        {
            dbContext.Add(caff);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Caff>> GetCaffsAsync() 
            => await dbContext.Caffs
                .Include(caff => caff.Comments).ThenInclude(caffComment => caffComment.User)
                .Include(caff => caff.Ciffs).ThenInclude(ciff => ciff.Tags)
                .ToListAsync();

        public async Task<Caff> GetCaffByIdAsync(string caffId) 
            => await dbContext.Caffs
                .Include(caff => caff.Comments).ThenInclude(caffComment => caffComment.User)
                .Include(caff => caff.Ciffs).ThenInclude(ciff => ciff.Tags)
                .SingleOrDefaultAsync(caff => caff.Id == caffId);

        public async Task<IEnumerable<Caff>> SearchCaffsByCaption(string query)
            => await dbContext.Caffs
                .Include(caff => caff.Comments).ThenInclude(caffComment => caffComment.User)
                .Include(caff => caff.Ciffs).ThenInclude(ciff => ciff.Tags)
                .Where(caff => caff.Ciffs
                                    .Select(ciff => ciff.Caption)
                                    .Any(caption => caption.ToLower().Contains(query.ToLower())))
                .ToListAsync();

        public async Task<IEnumerable<Caff>> SearchCaffsByTags(List<string> queryTags) 
            => await dbContext.Caffs
                .Include(caff => caff.Comments).ThenInclude(caffComment => caffComment.User)
                .Include(caff => caff.Ciffs).ThenInclude(ciff => ciff.Tags)
                .Where(caff => caff.Ciffs
                                    .SelectMany(ciff => ciff.Tags)
                                    .Select(tag => tag.Value)
                                    .Any(tag => queryTags.Contains(tag)))
                .ToListAsync();

        public async Task DeleteCaffAsync(Caff caff)
        {
            dbContext.Remove(caff);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddCaffCommentAsync(CaffComment caffComment)
        {
            await dbContext.CaffComments.AddAsync(caffComment);
            await dbContext.SaveChangesAsync();
        }

        public async Task<CaffComment> GetCaffCommentByIdAsync(string id) 
            => await dbContext.CaffComments.SingleOrDefaultAsync(caffComment => caffComment.Id == id);

        public async Task UpdateCaffCommentAsync(CaffComment caffComment)
        {
            dbContext.Update(caffComment);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteCaffCommentAsync(CaffComment caffComment)
        {
            dbContext.Remove(caffComment);
            await dbContext.SaveChangesAsync();
        }
    }
}
