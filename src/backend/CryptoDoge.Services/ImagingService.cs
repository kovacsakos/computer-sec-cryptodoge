using CryptoDoge.Shared;
using CryptoDoge.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CryptoDoge.Services
{


    public class ImagingService : IImagingService
    {
        private readonly ILogger<ImagingService> logger;
        private readonly IConfiguration configuration;
        private readonly string BasePath = string.Empty;

        public ImagingService(ILogger<ImagingService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;

            BasePath = configuration["Imaging:BasePath"];
        }

        public IEnumerable<string> SaveCaffImages(Caff caff)
        {
            using var loggerScope = new LoggerScope(logger);
            var ciffs = caff.Ciffs;
            if (ciffs.Count == 0)
            {
                return Enumerable.Empty<string>();
            }

            var filenames = new ConcurrentBag<string>();
            Parallel.ForEach(ciffs, ciff => filenames.Add(SaveCiff(ciff)));

            return filenames;
        }

        private string SaveCiff(Ciff ciff)
        {
            using var loggerScope = new LoggerScope(logger);
            var pixels = ciff.Pixels;
            using var bmp = new DirectBitmap(ciff.Width, ciff.Height);

            for (int x = 0; x < pixels.Count; x++)
            {
                for (int y = 0; y < pixels[x].Count; y++)
                {
                    bmp.SetPixel(y, x, Color.FromArgb(pixels[x][y][0], pixels[x][y][1], pixels[x][y][2]));
                }
            }

            var imageName = $"{ciff.Id}.png";
            var path = Path.GetFullPath(BasePath);
            Directory.CreateDirectory(path);

            path = Path.Combine(path, imageName);
            bmp.Bitmap.Save(path);
            return path;
        }
    }
}
