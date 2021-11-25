using CryptoDoge.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.Model.Interfaces
{
    public interface ICaffRepository
    {
        public Task AddNewCaffAsync(Caff caff);
    }
}
