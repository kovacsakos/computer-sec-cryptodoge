using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
        {
            return await dbContext.Caffs
                .Include(x => x.Comments).ThenInclude(x => x.User)
                .Include(x => x.Ciffs).ThenInclude(y => y.Tags)
                .ToListAsync();
        }

        public async Task<Caff> GetCaffByIdAsync(string caffId)
        {
            return await dbContext.Caffs
                .Include(x => x.Comments).ThenInclude(x => x.User)
                .Include(x => x.Ciffs).ThenInclude(y => y.Tags)
                .SingleOrDefaultAsync(c => c.Id == caffId);
        }

        public async Task DeleteCaffAsync(string caffId)
        {
            var caff = await GetCaffByIdAsync(caffId);
            if (caff != null)
            {
                dbContext.Remove(caff);
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
