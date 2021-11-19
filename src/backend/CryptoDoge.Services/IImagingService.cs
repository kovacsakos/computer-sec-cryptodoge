using CryptoDoge.Shared.Models;
using System.Collections.Generic;

namespace CryptoDoge.Services
{
    public interface IImagingService
    {
        IEnumerable<string> SaveCaffImages(Caff caff);
    }
}