using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
