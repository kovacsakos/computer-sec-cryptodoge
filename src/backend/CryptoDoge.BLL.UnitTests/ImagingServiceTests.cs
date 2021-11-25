using AutoMapper;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.BLL.Services;
using CryptoDoge.DAL;
using CryptoDoge.DAL.Repositories;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Interfaces;
using CryptoDoge.ParserService;
using CryptoDoge.Services.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.UnitTests
{
    public class ImagingServiceTests
    {
        private IImagingService imagingService;
        private readonly IParserService parserService = new ParserService.ParserService(Mock.Of<ILogger<ParserService.ParserService>>());
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private ApplicationDbContext dbContext;
        private ICaffRepository caffRepo;

        public ImagingServiceTests()
        {

            var configValues = new Dictionary<string, string> { { "Imaging:BasePath", "images" } };
            configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfiles());
            });
            mapper = mappingConfig.CreateMapper();
        }

        [SetUp]
        public async Task SetupAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: "CryptoDogeTestDb").Options;
            dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureDeletedAsync();
            caffRepo = new CaffRepository(dbContext);
            imagingService = new ImagingService(new ConsoleLogger<ImagingService>(), configuration, caffRepo, mapper);
        }

        [Test]
        public async Task SaveCaffImages_1caffAsync()
        {
            ParsedCaff caff = parserService.GetCaffFromFile(@"TestData\1.caff");
            var fileNames = (await imagingService.SaveCaffImagesAsync(caff)).ToList();

            Assert.AreEqual(caff.Num_anim, fileNames.Count);

            var basePath = Path.GetFullPath(configuration["Imaging:BasePath"]);
            foreach (var fileName in fileNames)
            {
                var fullPath = Path.Combine(basePath, fileName);
                Assert.IsTrue(File.Exists(fullPath));
                Assert.IsNotNull(caff.Ciffs.Find(c => fileName.Contains(c.Id)));
                File.Delete(fullPath);
            }

            var savedCaffs = await dbContext.Caffs.Include(c => c.Ciffs).ThenInclude(ci => ci.Tags).ToListAsync();
            Assert.AreEqual(1, savedCaffs.Count);
            CollectionAssert.AreEquivalent(fileNames, savedCaffs[0].Ciffs.Select(ci => $"{ci.Id}.png"));
            ValidateCaff(savedCaffs[0]);
        }

        [Test]
        public async Task SaveCaffImages_EmtpyCaff()
        {
            ParsedCaff caff = parserService.GetCaffFromFile(@"TestData\1.caff");
            caff.Ciffs.Clear();
            caff.Num_anim = 0;

            var fileNames = (await imagingService.SaveCaffImagesAsync(caff)).ToList();

            Assert.AreEqual(caff.Num_anim, fileNames.Count);
            Assert.IsEmpty(fileNames);

            var savedCaffs = await dbContext.Caffs.Include(c => c.Ciffs).ThenInclude(ci => ci.Tags).ToListAsync();
            Assert.AreEqual(1, savedCaffs.Count);
            Assert.IsEmpty(savedCaffs[0].Ciffs.ToList());

        }

        private void ValidateCaff(Caff caff)
        {
            Assert.AreEqual("Test Creator", caff.Creator);
            Assert.AreEqual(2, caff.NumberOfAnimations);
            Assert.AreEqual(2, caff.Ciffs.Count);

            Assert.AreEqual(1000, caff.Ciffs.ToList()[0].Width);
            Assert.AreEqual(667, caff.Ciffs.ToList()[0].Height);

            Assert.AreEqual(new DateTime(2020, 7, 2, 14, 50, 0), caff.CreationDate);

            Assert.AreEqual("Beautiful scenery", caff.Ciffs.ToList()[0].Caption);

            var expectedTags = new List<string>() { "mountains", "sunset", "landscape" };
            CollectionAssert.AreEquivalent(expectedTags, caff.Ciffs.ToList()[0].Tags.Select(t => t.Value));
        }

    }
}