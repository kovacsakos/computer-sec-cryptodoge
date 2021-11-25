using AutoMapper;
using CryptoDoge.BLL.Dtos;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Exceptions;
using CryptoDoge.Model.Interfaces;
using CryptoDoge.ParserService;
using CryptoDoge.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Services
{
    public class ImagingService : IImagingService
    {
        private readonly ILogger<ImagingService> logger;
        private readonly string BasePath = string.Empty;
        private readonly ICaffRepository caffRepository;
        private readonly IMapper mapper;

        public ImagingService(ILogger<ImagingService> logger, IConfiguration configuration, ICaffRepository caffRepository, IMapper mapper)
        {
            this.logger = logger;
            BasePath = configuration["Imaging:BasePath"];
            this.caffRepository = caffRepository;
            this.mapper = mapper;
        }

        public async Task<CaffDto> SaveCaffImagesAsync(ParsedCaff parsedCaff, User user)
        {
            using var loggerScope = new LoggerScope(logger);

            var newCaff = mapper.Map<Caff>(parsedCaff);
            newCaff.Id = Guid.NewGuid().ToString();

            var ciffs = parsedCaff.Ciffs;
            if (ciffs.Count == 0)
            {
                await caffRepository.AddNewCaffAsync(newCaff);
                return await GetCaffByIdAsync(newCaff.Id);
            }

            var newCiffs = new ConcurrentBag<Ciff>();
            Parallel.ForEach(ciffs, ciff => newCiffs.Add(SaveCiff(ciff)));

            newCaff.Ciffs = newCiffs.ToList();
            await caffRepository.AddNewCaffAsync(newCaff);
            return await GetCaffByIdAsync(newCaff.Id);
        }

        public async Task<CaffDto> GetCaffByIdAsync(string id)
        {
            using var loggerScope = new LoggerScope(logger);
            var caff = await caffRepository.GetCaffByIdAsync(id);
            return mapper.Map<CaffDto>(caff);
        }

        public async Task DeleteCaffImagesAsync(string id, User user)
        {
            using var loggerScope = new LoggerScope(logger);
            var caff = await caffRepository.GetCaffByIdAsync(id);

            if (caff != null)
            {
                var path = Path.GetFullPath(BasePath);
                foreach (var ciff in caff.Ciffs)
                {
                    var imageName = $"{ciff.Id}.png";
                    var imagePath = Path.Combine(path, imageName);
                    File.Delete(imagePath);
                }

                await caffRepository.DeleteCaffAsync(caff);
            }
        }

        public async Task CommentOnCaff(CaffCommentDto caffCommentDto, User user)
        {
            using var loggerScope = new LoggerScope(logger);
            var caff = await caffRepository.GetCaffByIdAsync(caffCommentDto.CaffId);
            if (caff != null)
            {
                await caffRepository.AddComment(new CaffComment
                {
                    Comment = caffCommentDto.Comment,
                });
            } 
            else
            {
                throw new NotFoundException("Caff does not exists.", 404);
            }
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

            path = Path.Combine(path, imageName);
            bmp.Bitmap.Save(path);

            var newCiff = mapper.Map<Ciff>(parsedCiff);
            newCiff.Id = parsedCiff.Id;

            return newCiff;
        }
    }
}
