using AutoMapper;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Interfaces;
using CryptoDoge.ParserService;
using CryptoDoge.Shared;
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

namespace CryptoDoge.BLL.Services
{
    public class ImagingService : IImagingService
    {
        private readonly ILogger<ImagingService> logger;
        private readonly IConfiguration configuration;
        private readonly string BasePath = string.Empty;
        private readonly ICaffRepository caffRepository;
        private readonly IMapper mapper;

        public ImagingService(ILogger<ImagingService> logger, IConfiguration configuration, ICaffRepository caffRepository, IMapper mapper)
        {
            this.logger = logger;
            this.configuration = configuration;

            BasePath = configuration["Imaging:BasePath"];
            this.caffRepository = caffRepository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<string>> SaveCaffImagesAsync(ParsedCaff parsedCaff)
        {
            using var loggerScope = new LoggerScope(logger);

            var newCaff = mapper.Map<Caff>(parsedCaff);
            newCaff.Id = Guid.NewGuid().ToString();

            var ciffs = parsedCaff.Ciffs;
            if (ciffs.Count == 0)
            {
                await caffRepository.AddNewCaffAsync(newCaff);
                return Enumerable.Empty<string>();
            }

            var newCiffs = new ConcurrentBag<Ciff>();
            Parallel.ForEach(ciffs, ciff => newCiffs.Add(SaveCiff(ciff)));

            newCaff.Ciffs = newCiffs.ToList();
            await caffRepository.AddNewCaffAsync(newCaff);
            return newCiffs.Select(c => $"{c.Id}.png");
        }

        private Ciff SaveCiff(ParsedCiff parsedCiff)
        {
            using var loggerScope = new LoggerScope(logger);
            var pixels = parsedCiff.Pixels;
            using var bmp = new DirectBitmap(parsedCiff.Width, parsedCiff.Height);

            for (int x = 0; x < pixels.Count; x++)
            {
                for (int y = 0; y < pixels[x].Count; y++)
                {
                    bmp.SetPixel(y, x, Color.FromArgb(pixels[x][y][0], pixels[x][y][1], pixels[x][y][2]));
                }
            }

            var imageName = $"{parsedCiff.Id}.png";
            var path = Path.GetFullPath(BasePath);
            Directory.CreateDirectory(path);

            path = Path.Combine(path, imageName);
            bmp.Bitmap.Save(path);

            var newCiff = mapper.Map<Ciff>(parsedCiff);
            newCiff.Id = parsedCiff.Id;

            return newCiff;
        }
    }
}
