using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.ParserService.UnitTests
{
    public class Tests
    {
        private static IParserService parserService;

        [SetUp]
        public void Setup()
        {
            parserService = new ParserService(Mock.Of<ILogger<ParserService>>());
        }

        [Test]
        public async Task GetCaffFromByteArray_1caff()
        {
            var caffFile = await File.ReadAllBytesAsync(@"TestData\1.caff");

            var caff = parserService.GetCaffFromByteArray(caffFile);

            ValidateCaff(caff);
        }

        [Test]
        public void GetCaffFromByteArray_EmptyArray()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var caff = parserService.GetCaffFromByteArray(Array.Empty<byte>());
            });
        }

        [Test]
        public void GetCaffFromFile_1caff()
        {
            var caff = parserService.GetCaffFromFile(Path.GetFullPath(@"TestData\1.caff"));

            ValidateCaff(caff);
        }

        [Test]
        public void GetCaffFromFile_FileNotFound()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                var caff = parserService.GetCaffFromFile(Path.GetFullPath(@"TestData\11.caff"));
            });
        }

        [Test]
        public async Task GetCaffFromMemoryStream()
        {
            using Stream fileStream = File.OpenRead(@"TestData\1.caff");
            using var ms = new MemoryStream();

            await fileStream.CopyToAsync(ms);

            var caff = parserService.GetCaffFromMemoryStream(ms);

            ValidateCaff(caff);
        }

        private void ValidateCaff(ParsedCaff caff)
        {
            Assert.AreEqual("Test Creator", caff.Creator);
            Assert.AreEqual(2, caff.Num_anim);
            Assert.AreEqual(2, caff.Ciffs.Count);

            Assert.AreEqual(1000, caff.Ciffs[0].Width);
            Assert.AreEqual(667, caff.Ciffs[0].Height);

            Assert.AreEqual(new DateTime(2020, 7, 2, 14, 50, 0), caff.CreationDate);

            Assert.AreEqual("Beautiful scenery", caff.Ciffs[0].Caption);

            var expectedTags = new List<string>() { "mountains", "sunset", "landscape" };
            CollectionAssert.AreEquivalent(expectedTags, caff.Ciffs[0].Tags);
        }
    }
}