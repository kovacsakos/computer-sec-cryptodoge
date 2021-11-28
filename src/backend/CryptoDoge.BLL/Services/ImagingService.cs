using AutoMapper;
using CryptoDoge.BLL.Dtos;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Exceptions;
using CryptoDoge.Model.Interfaces;
using CryptoDoge.ParserService;
using CryptoDoge.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Services
{
    public class ImagingService : IImagingService
    {
        private readonly ILogger<ImagingService> logger;
        private readonly string BasePath;
        private readonly ICaffRepository caffRepository;
        private readonly IMapper mapper;
        private readonly string rawFolderPath;

        public ImagingService(ILogger<ImagingService> logger, IConfiguration configuration, ICaffRepository caffRepository, IMapper mapper)
        {
            this.logger = logger;
            BasePath = configuration["Imaging:BasePath"];
            rawFolderPath = Path.Combine(BasePath, "raw");
            this.caffRepository = caffRepository;
            this.mapper = mapper;
        }
        public async Task<CaffDto> SaveCaffAsync(ParsedCaff parsedCaff, User user, IFormFile rawfile)
        {
            var result = await SaveCaffImagesAsync(parsedCaff, user);

            if (rawfile.Length > 0)
            {
                var rawFolderPath = Path.Combine(BasePath, "raw");
                if (!Directory.Exists(rawFolderPath))
                {
                    Directory.CreateDirectory(rawFolderPath);
                }

                string filePath = Path.Combine(rawFolderPath, $"{result.Id}.caff");
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await rawfile.CopyToAsync(fileStream);
                }
            }

            return result;
        }

        public async Task<CaffDto> SaveCaffImagesAsync(ParsedCaff parsedCaff, User user)
        {
            using var loggerScope = new LoggerScope(logger);

            var newCaff = mapper.Map<Caff>(parsedCaff);
            newCaff.Id = Guid.NewGuid().ToString();
            newCaff.UploadedBy = user;

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

        public async Task<IEnumerable<CaffDto>> GetCaffsAsync()
        {
            using var loggerScope = new LoggerScope(logger);
            var caffs = await caffRepository.GetCaffsAsync();
            return caffs.Select(caff => mapper.Map<CaffDto>(caff)).ToList();
        }

        public async Task<CaffDto> GetCaffByIdAsync(string caffId)
        {
            using var loggerScope = new LoggerScope(logger);
            var caff = await caffRepository.GetCaffByIdAsync(caffId);
            return mapper.Map<CaffDto>(caff);
        }

        public async Task<IEnumerable<CaffDto>> SearchCaffsByCaption(string query)
        {
            using var loggerScope = new LoggerScope(logger);
            var result = await caffRepository.SearchCaffsByCaption(query);
            return result.Select(caff => mapper.Map<CaffDto>(caff)).ToList();
        }

        public async Task<IEnumerable<CaffDto>> SearchCaffsByTags(List<string> queryTags)
        {
            using var loggerScope = new LoggerScope(logger);
            var result = await caffRepository.SearchCaffsByTags(queryTags);
            return result.Select(caff => mapper.Map<CaffDto>(caff)).ToList();
        }

        public async Task DeleteCaffImagesAsync(string caffId)
        {
            using var loggerScope = new LoggerScope(logger);
            var caff = await caffRepository.GetCaffByIdAsync(caffId);

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

        public async Task<string> AddCaffCommentAsync(string caffId, string comment, User user)
        {
            using var loggerScope = new LoggerScope(logger);
            var caff = await caffRepository.GetCaffByIdAsync(caffId);
            if (caff != null)
            {
                var id = Guid.NewGuid().ToString();
                await caffRepository.AddCaffCommentAsync(new CaffComment
                {
                    Id = id,
                    Comment = comment,
                    Caff = caff,
                    User = user,
                });
                return id;
            }
            else
            {
                throw new NotFoundException("Caff does not exists.", 404);
            }
        }

        public async Task<CaffCommentReturnDto> GetCaffCommentByIdAsync(string id)
        {
            using var loggerScope = new LoggerScope(logger);
            var caffComment = await caffRepository.GetCaffCommentByIdAsync(id);
            if (caffComment != null)
            {
                return mapper.Map<CaffCommentReturnDto>(caffComment);
            }
            else
            {
                throw new NotFoundException("Caff comment does not exists.", 404);
            }
        }

        public async Task UpdateCommentOnCaffAsync(string caffCommentId, string comment)
        {
            using var loggerScope = new LoggerScope(logger);
            var caffComment = await caffRepository.GetCaffCommentByIdAsync(caffCommentId);
            if (caffComment != null)
            {
                caffComment.Comment = comment;
                await caffRepository.UpdateCaffCommentAsync(caffComment);
            }
            else
            {
                throw new NotFoundException("Caff comment does not exists.", 404);
            }
        }

        public async Task DeleteCaffCommentAsync(string caffCommentId)
        {
            using var loggerScope = new LoggerScope(logger);
            var caffComment = await caffRepository.GetCaffCommentByIdAsync(caffCommentId);
            if (caffComment != null)
            {
                await caffRepository.DeleteCaffCommentAsync(caffComment);
            }
        }

        private Ciff SaveCiff(ParsedCiff parsedCiff)
        {
            using var loggerScope = new LoggerScope(logger);
            var pixels = parsedCiff.Pixels;
            using var bmp = new DirectBitmap(parsedCiff.Width, parsedCiff.Height);

            for (int y = 0; y < pixels.Count; y++)
            {
                for (int x = 0; x < pixels[y].Count; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(pixels[y][x][0], pixels[y][x][1], pixels[y][x][2]));
                }
            }

            var imageName = $"{parsedCiff.Id}.png";
            var path = Path.GetFullPath(BasePath);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, imageName);
            bmp.Bitmap.Save(path);

            var newCiff = mapper.Map<Ciff>(parsedCiff);
            newCiff.Id = parsedCiff.Id;

            return newCiff;
        }

        public async Task<byte[]> GetRawCaffByIdAsync(string caffId)
        {
            using var loggerScope = new LoggerScope(logger);
            var caff = await GetCaffByIdAsync(caffId);

            if(caff == null)
            {
                throw new NotFoundException("Caff does not exists.");
            }
            var fileName = Path.Combine(rawFolderPath, $"{caff.Id}.caff");
            BinaryReader binReader = new BinaryReader(File.Open(fileName, FileMode.Open));

            byte[] fileBytes;
            using (var stream = new MemoryStream())
            {
                binReader.BaseStream.CopyTo(stream);
                fileBytes = stream.ToArray();
            }

            return fileBytes;
        }
    }
}
