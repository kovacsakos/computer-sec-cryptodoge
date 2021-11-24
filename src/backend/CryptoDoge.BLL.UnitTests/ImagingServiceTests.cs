using CryptoDoge.BLL.Interfaces;
using CryptoDoge.BLL.Services;
using CryptoDoge.ParserService;
using CryptoDoge.Services.UnitTests.Helpers;
using CryptoDoge.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CryptoDoge.BLL.UnitTests
{
    public class ImagingServiceTests
    {
        private readonly IImagingService imagingService;
        private readonly IParserService parserService = new ParserService.ParserService(Mock.Of<ILogger<ParserService.ParserService>>());
        private readonly IConfiguration configuration;

        public ImagingServiceTests()
        {
            var configValues = new Dictionary<string, string> { { "Imaging:BasePath", "images" } };
            configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();
            imagingService = new ImagingService(new ConsoleLogger<ImagingService>(), configuration);
        }

        [Test]
        public void SaveCaffImages_1caff()
        {
            Caff caff = parserService.GetCaffFromFile(@"TestData\1.caff");
            var fileNames = imagingService.SaveCaffImages(caff).ToList();

            Assert.AreEqual(caff.Num_anim, fileNames.Count);

            var basePath = Path.GetFullPath(configuration["Imaging:BasePath"]);
            foreach (var fileName in fileNames)
            {
                Assert.IsTrue(File.Exists(Path.Combine(basePath, fileName)));
                Assert.IsNotNull(caff.Ciffs.Find(c => fileName.Contains(c.Id)));
                File.Delete(fileName);
            }
        }

        [Test]
        public void SaveCaffImages_EmtpyCaff()
        {
            Caff caff = parserService.GetCaffFromFile(@"TestData\1.caff");
            caff.Ciffs.Clear();
            caff.Num_anim = 0;

            var fileNames = imagingService.SaveCaffImages(caff).ToList();

            Assert.AreEqual(caff.Num_anim, fileNames.Count);
            Assert.IsEmpty(fileNames);
        }
    }
}