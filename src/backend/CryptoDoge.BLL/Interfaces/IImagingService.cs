using CryptoDoge.Shared.Models;
using System.Collections.Generic;

namespace CryptoDoge.BLL.Interfaces
{
    public interface IImagingService
    {
        IEnumerable<string> SaveCaffImages(Caff caff);
    }
}