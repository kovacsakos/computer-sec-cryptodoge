using AutoMapper;
using CryptoDoge.BLL.Dtos;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.BLL.Services;
using CryptoDoge.DAL;
using CryptoDoge.DAL.Repositories;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Exceptions;
using CryptoDoge.Model.Interfaces;
using CryptoDoge.ParserService;
using CryptoDoge.Services.UnitTests.Helpers;
using CryptoDoge.Shared;
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
        private ICaffRepository caffRepository;
        private User user;
        private ParsedCaff parsedCaff;
        private const string Comment = "Comment about something";

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
            caffRepository = new CaffRepository(dbContext);
            imagingService = new ImagingService(new ConsoleLogger<ImagingService>(), configuration, caffRepository, mapper);
            parsedCaff = parserService.GetCaffFromFile(@"TestData\1.caff");
            user = new User { Id = "c193a1f7-0000-0000-0000-976f4811bf5f" };
        }

        [Test]
        public async Task SaveCaffImages_1caffAsync()
        {
            
            var savedCaff = await imagingService.SaveCaffImagesAsync(parsedCaff, user);

            Assert.AreEqual(parsedCaff.Num_anim, savedCaff.NumberOfAnimations);

            var basePath = Path.GetFullPath(configuration["Imaging:BasePath"]);
            foreach (var ciff in savedCaff.Ciffs)
            {
                var fullPath = Path.Combine(basePath, $"{ciff.Id}.png");
                Assert.IsTrue(File.Exists(fullPath));
                Assert.IsNotNull(parsedCaff.Ciffs.Find(c => ciff.Id == c.Id));
                File.Delete(fullPath);
            }

            var savedCaffs = await dbContext.Caffs.Include(c => c.Ciffs).ThenInclude(ci => ci.Tags).ToListAsync();
            Assert.AreEqual(1, savedCaffs.Count);
            CollectionAssert.AreEquivalent(savedCaff.Ciffs.Select(x => x.Id), savedCaffs[0].Ciffs.Select(ci => ci.Id));
            ValidateCaff(savedCaffs[0]);
        }

        [Test]
        public async Task SaveCaffImages_EmtpyCaff()
        {
            parsedCaff.Ciffs.Clear();
            parsedCaff.Num_anim = 0;

            var savedCaff = await imagingService.SaveCaffImagesAsync(parsedCaff, user);

            Assert.AreEqual(parsedCaff.Num_anim, savedCaff.NumberOfAnimations);
            Assert.IsEmpty(savedCaff.Ciffs);

            var savedCaffs = await dbContext.Caffs.Include(c => c.Ciffs).ThenInclude(ci => ci.Tags).ToListAsync();
            Assert.AreEqual(1, savedCaffs.Count);
            Assert.IsEmpty(savedCaffs[0].Ciffs.ToList());

        }

        [Test]
        public async Task GetCaffsAsync_NoResult()
        {
            var result = await imagingService.GetCaffsAsync();
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetCaffsAsync_HasResult()
        {
            var savedCaff = await imagingService.SaveCaffImagesAsync(parsedCaff, user);

            parsedCaff.Ciffs.ForEach(x => x.Id = Guid.NewGuid().ToString());
            var savedCaff2 = await imagingService.SaveCaffImagesAsync(parsedCaff, user);
            var result = await imagingService.GetCaffsAsync();

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.ToList().SequenceEqual(new List<CaffDto> { savedCaff, savedCaff2 }, CaffDtoEquals));
        }

        [Test]
        public async Task GetCaffByIdAsync_NoResult()
        {
            await imagingService.SaveCaffImagesAsync(parsedCaff, user);
            var result = await imagingService.GetCaffByIdAsync("notexistingcaffid");
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCaffByIdAsync_HasResult()
        {
            var savedCaff = await imagingService.SaveCaffImagesAsync(parsedCaff, user);
            var result = await imagingService.GetCaffByIdAsync(savedCaff.Id);
            Assert.IsNotNull(result);
            Assert.IsTrue(CaffDtoEquals(savedCaff, result));
        }

        [Test]
        public async Task DeleteCaffImagesAsync()
        {
            var savedCaff = await imagingService.SaveCaffImagesAsync(parsedCaff, user);
            await imagingService.DeleteCaffImagesAsync(savedCaff.Id);
            var basePath = Path.GetFullPath(configuration["Imaging:BasePath"]);
            foreach (var ciff in savedCaff.Ciffs)
            {
                var fullPath = Path.Combine(basePath, $"{ciff.Id}.png");
                Assert.IsFalse(File.Exists(fullPath));
            }     
        }

        [Test]
        public async Task AddCaffCommentAsync_ToNotExistingCaff()
        {
            var exception = await Task.FromResult(
                Assert.ThrowsAsync<NotFoundException>(async () => await imagingService.AddCaffCommentAsync("notexistingcaffid", Comment, user)));        
            Assert.AreEqual("Caff does not exists.", exception.Message);
        }

        [Test]
        public async Task AddAndGetCaffCommentAsync()
        {
            var savedCaff = await imagingService.SaveCaffImagesAsync(parsedCaff, user);
            var commentId = await imagingService.AddCaffCommentAsync(savedCaff.Id, Comment, user);
            var dbComment = await imagingService.GetCaffCommentByIdAsync(commentId);
            Assert.IsNotNull(dbComment);
            Assert.IsTrue(CaffCommentEquals(new CaffComment
            {
                User = user,
                Id = commentId,
                Comment = Comment,
                Caff = new Caff { Id = savedCaff.Id },
            }, dbComment));
        }

        [Test]
        public async Task GetCaffCommentByIdAsync_NoResult()
        {
            var exception = await Task.FromResult(
                Assert.ThrowsAsync<NotFoundException>(async () => await imagingService.GetCaffCommentByIdAsync("notexistingcommentid")));
            Assert.AreEqual("Caff comment does not exists.", exception.Message);
        }

        [Test]
        public async Task DeleteCaffCommentAsync()
        {
            var savedCaff = await imagingService.SaveCaffImagesAsync(parsedCaff, user);
            var commentId = await imagingService.AddCaffCommentAsync(savedCaff.Id, Comment, user);
            await imagingService.DeleteCaffCommentAsync(commentId);
            var exception = await Task.FromResult(
                Assert.ThrowsAsync<NotFoundException>(async () => await imagingService.GetCaffCommentByIdAsync(commentId)));
            Assert.AreEqual("Caff comment does not exists.", exception.Message);
        }

        [Test]
        public async Task UpdateCommentOnCaffAsync_NotExistingComment()
        {
            var exception = await Task.FromResult(
                Assert.ThrowsAsync<NotFoundException>(async () => await imagingService.UpdateCommentOnCaffAsync("notexistingcommentid", Comment)));
            Assert.AreEqual("Caff comment does not exists.", exception.Message);
        }

        [Test]
        public async Task UpdateCommentOnCaffAsync_Ok()
        {
            var savedCaff = await imagingService.SaveCaffImagesAsync(parsedCaff, user);
            var commentId = await imagingService.AddCaffCommentAsync(savedCaff.Id, Comment, user);
            await imagingService.UpdateCommentOnCaffAsync(commentId, "Modified comment");
            var dbComment = await imagingService.GetCaffCommentByIdAsync(commentId);
            Assert.AreEqual("Modified comment", dbComment.Comment);
        }

        [Test]
        public async Task SearchCaffsByCaption_NoResult()
        {
            var result = await imagingService.SearchCaffsByCaption("notexistingcaption");
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task SearchCaffsByCaption_HasResult()
        {
            var caption = "Beautiful scenery";
            await imagingService.SaveCaffImagesAsync(parsedCaff, user);
            var result = await imagingService.SearchCaffsByCaption(caption);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(caption, result.Single().Captions.Single());
        }

        [Test]
        public async Task SearchCaffsByTags_NoResult()
        {
            var result = await imagingService.SearchCaffsByTags(new List<string> { "notexistingtag1", "notexistingtag2", "notexistingtag3" });
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task SearchCaffsByTags_HasResult_OneTag()
        {
            var expectedTags = new List<string>() { "mountains", "sunset", "landscape" };
            await imagingService.SaveCaffImagesAsync(parsedCaff, user);

            foreach (var tag in expectedTags)
            {
                var result = await imagingService.SearchCaffsByTags(new List<string> { tag });
                Assert.IsNotEmpty(result);
                Assert.IsTrue(result.Single().Tags.Contains(tag));
            }
        }

        [Test]
        public async Task SearchCaffsByTags_HasResult_MoreTags()
        {
            var expectedTags = new List<string>() { "mountains", "sunset", "landscape" };
            await imagingService.SaveCaffImagesAsync(parsedCaff, user);
            var result = await imagingService.SearchCaffsByTags(expectedTags);
            Assert.IsNotEmpty(result);
            Assert.IsTrue(expectedTags.Any(tag => result.Single().Tags.Contains(tag)));
        }

        private static bool CaffDtoEquals(CaffDto x, CaffDto y)
        {
            if (!CaffEqualsWithoutSequenceProperties(x, y))
                return false;

            return x.Ciffs.ToList().SequenceEqual(y.Ciffs.ToList(), CiffDtoEquals) &&
                   x.Comments.ToList().SequenceEqual(y.Comments.ToList(), CaffCommentEquals) &&
                   x.Captions.ToList().SequenceEqual(y.Captions.ToList()) &&
                   x.Tags.ToList().SequenceEqual(y.Tags.ToList());
        }

        private static bool CaffEqualsWithoutSequenceProperties(CaffDto x, CaffDto y)
            => EqualityComparerHelper.PropertiesEqual(x, y, nameof(CaffDto.Ciffs), nameof(CaffDto.Captions), nameof(CaffDto.Comments), nameof(CaffDto.Tags));

        private static bool CiffDtoEquals(CiffDto x, CiffDto y) 
            => EqualityComparerHelper.PropertiesEqual(x, y);

        private static bool CaffCommentEquals(CaffComment x, CaffComment y)
        {
            if (!CaffCommentEqualsWithoutUserAndCaff(x, y))
                return false;

            return x.Caff.Id == y.Caff.Id && x.User.Id == y.User.Id;
        }

        private static bool CaffCommentEqualsWithoutUserAndCaff(CaffComment x, CaffComment y)
            => EqualityComparerHelper.PropertiesEqual(x, y, nameof(CaffComment.User), nameof(CaffComment.Caff));

        private static void ValidateCaff(Caff caff)
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