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

namespace CryptoDoge.Services.UnitTests
{
    public class ImagingServiceTests
    {
        private readonly IImagingService imagingService;
        private readonly IParserService parserService = new ParserService.ParserService(Mock.Of<ILogger<ParserService.ParserService>>());

        public ImagingServiceTests()
        {
            var myConfiguration = new Dictionary<string, string> {{"Imaging:BasePath", "images"}};
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(myConfiguration).Build();
            imagingService = new ImagingService(new ConsoleLogger<ImagingService>(), configuration);
        }

        [Test]
        public void SaveCaffImages()
        {
            Caff caff = parserService.GetCaffFromFile(@"TestData\1.caff");
            var filenames = imagingService.SaveCaffImages(caff).ToList();

            Assert.AreEqual(caff.Num_anim, filenames.Count);

            foreach (var item in filenames)
            {
                Assert.IsTrue(File.Exists(item));
                File.Delete(item);
            }
        }
    }
}