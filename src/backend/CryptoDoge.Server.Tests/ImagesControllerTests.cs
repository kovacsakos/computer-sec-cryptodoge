using AutoMapper;
using CryptoDoge.BLL;
using CryptoDoge.BLL.Dtos;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.BLL.Services;
using CryptoDoge.DAL;
using CryptoDoge.DAL.Repositories;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Interfaces;
using CryptoDoge.ParserService;
using CryptoDoge.Server.Controllers;
using CryptoDoge.Server.Infrastructure.Services;
using CryptoDoge.Server.Tests.Helpers;
using CryptoDoge.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CryptoDoge.Server.Tests
{
    public class ImagesControllerTests
    {
        private IImagingService imagingService;
        private readonly IParserService parserService = new ParserService.ParserService(Mock.Of<ILogger<ParserService.ParserService>>());
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private ICaffRepository caffRepository;
        private ApplicationDbContext dbContext;
        private IdentityService identityService;
        private Mock<UserManager<User>> userManager;
        private ImagesController imagesController;

        public ImagesControllerTests()
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
            var _store = new Mock<IUserStore<User>>();
            userManager = new Mock<UserManager<User>>(_store.Object, null, null, null, null, null, null, null, null);

            var user = new User() { UserName = "JohnDoe", Id = "30776ea1-016e-419d-becf-afeab0c548e7" };
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockPrincipal = new Mock<IPrincipal>();
            mockPrincipal.Setup(x => x.Identity).Returns(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);

            identityService = new IdentityService(new HttpContextAccessor { HttpContext = mockHttpContext.Object }, userManager.Object);
            imagesController = new ImagesController(parserService, imagingService, identityService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }
            };
        }

        [Test]
        public async Task GetCaffsAsync_NoResult()
        {
            var response = (await imagesController.GetCaffs());
            Assert.IsInstanceOf<OkObjectResult>(response.Result);
            Assert.IsEmpty((response.Result as OkObjectResult).Value as List<CaffDto>);
        }

        [Test]
        public async Task GetCaffsAsync_AfterUpload()
        {
            var caffDto = await UploadCaff();
            var response = (await imagesController.GetCaffs());
            var responseResult = response.Result;
            var caffList = (responseResult as OkObjectResult).Value as List<CaffDto>;

            Assert.IsInstanceOf<OkObjectResult>(responseResult);
            Assert.IsNotEmpty(caffList);
            Assert.AreEqual(caffDto.Id, caffList.Single().Id);

            await CleanUp(caffDto.Id);
        }

        [Test]
        public async Task GetCaffByIdAsync_NoResult()
        {
            var response = (await imagesController.GetCaffByIdAsync("notexisting")).Result;
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task GetCaffByIdAsync_AfterUpload()
        {
            var caffDto = await UploadCaff();
            var response = await imagesController.GetCaffByIdAsync(caffDto.Id);
            Assert.IsInstanceOf<OkObjectResult>(response.Result);
            var responseResult = (response.Result as OkObjectResult).Value;
            var caffResult = responseResult as CaffDto;

            Assert.IsNotNull(caffResult);
            Assert.AreEqual(caffDto.Id, caffResult.Id);

            await CleanUp(caffDto.Id);
        }

        [Test]
        public async Task DownloadCaffByIdAsync_NoResult()
        {
            var response = (await imagesController.DownloadCaff("notexisting"));
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task DownloadCaffByIdAsync_AfterUpload()
        {
            var caffDto = await UploadCaff();
            var response = await imagesController.DownloadCaff(caffDto.Id);
            Assert.IsInstanceOf<FileContentResult>(response);
            var responseResult = (response as FileContentResult);
            var caffResultName = responseResult.FileDownloadName;

            Assert.IsNotNull(responseResult);
            Assert.IsTrue(responseResult.FileContents.Length > 0);
            Assert.AreEqual(caffDto.Id+".caff", caffResultName);

            await CleanUp(caffDto.Id);
        }

        [Test]
        public async Task GetCaffComment_NotExist()
        {
            var response = (await imagesController.GetComment("id"));
            Assert.IsInstanceOf<BadRequestObjectResult>(response.Result);
            Assert.AreEqual("Caff comment does not exists.", (response.Result as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task UpdateCaffComment_NotExist()
        {
            var response = await imagesController.UpdateComment("id", new CaffCommentDto { Comment = "Update" });
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            Assert.AreEqual("Caff comment does not exists.", (response as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task GetCaffComment_AfterUpload()
        {
            var caffDto = await UploadCaff();
            var commentResponse = await imagesController.PostComment(caffDto.Id, new CaffCommentDto { Comment = "Valami" });
            Assert.IsInstanceOf<OkObjectResult>(commentResponse.Result);
            var commentResult = (commentResponse.Result as OkObjectResult).Value;
            var commentId = commentResult as string;

            var comment = await imagesController.GetComment(commentId);
            Assert.IsInstanceOf<OkObjectResult>(comment.Result);
            var caffCommentResult = (comment.Result as OkObjectResult).Value;
            var caffComment = caffCommentResult as CaffCommentReturnDto;

            Assert.IsNotNull(caffComment);
            Assert.AreEqual("Valami", caffComment.Comment);

            await CleanUp(caffDto.Id);
        }

        [Test]
        public async Task UpdateCaffComment_AfterUpload()
        {
            var caffDto = await UploadCaff();
            var commentResponse = await imagesController.PostComment(caffDto.Id, new CaffCommentDto { Comment = "Valami" });
            Assert.IsInstanceOf<OkObjectResult>(commentResponse.Result);
            var commentResult = (commentResponse.Result as OkObjectResult).Value;
            var commentId = commentResult as string;

            var updateResult = await imagesController.UpdateComment(commentId, new CaffCommentDto { Comment = "Update" });
            Assert.IsInstanceOf<OkResult>(updateResult);

            var comment = await imagesController.GetComment(commentId);
            Assert.IsInstanceOf<OkObjectResult>(comment.Result);

            var caffCommentResult = (comment.Result as OkObjectResult).Value;
            var caffComment = caffCommentResult as CaffCommentReturnDto;

            Assert.IsNotNull(caffComment);
            Assert.AreEqual("Update", caffComment.Comment);

            await CleanUp(caffDto.Id);
        }

        [Test]
        public async Task DeleteCaffAsync()
        {
            var response = await imagesController.DeleteCaff("id");
            Assert.IsInstanceOf<NoContentResult>(response);
        }

        [Test]
        public async Task UploadAsync()
        {
            var caffDto = await UploadCaff();
            Assert.IsInstanceOf<CaffDto>(caffDto);
            Assert.IsNotNull(caffDto);
            await CleanUp(caffDto.Id);
        }

        [Test]
        public async Task PostComment_CaffNotExists()
        {
            var commentResponse = await imagesController.PostComment("notexistingcaffid", new CaffCommentDto { Comment = "Valami" });
            Assert.IsInstanceOf<BadRequestObjectResult>(commentResponse.Result);
            Assert.AreEqual("Caff does not exists.", (commentResponse.Result as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task PostComment_AfterUpload()
        {
            var caffDto = await UploadCaff();
            var commentResponse = await imagesController.PostComment(caffDto.Id, new CaffCommentDto { Comment = "Valami" });
            Assert.IsInstanceOf<OkObjectResult>(commentResponse.Result);

            var commentResult = (commentResponse.Result as OkObjectResult).Value;
            var commentId = commentResult as string;

            Assert.IsNotNull(commentId);
            Assert.IsNotEmpty(commentId);

            await CleanUp(caffDto.Id);
        }

        [Test]
        public async Task DeleteComment_NotExists()
        {
            var response = await imagesController.DeleteComment("notexistingid");
            Assert.IsInstanceOf<NoContentResult>(response);
        }

        [Test]
        public async Task DeleteComment_AfterUpload()
        {
            var caffDto = await UploadCaff();
            var commentResponse = await imagesController.PostComment(caffDto.Id, new CaffCommentDto { Comment = "Valami" });
            Assert.IsInstanceOf<OkObjectResult>(commentResponse.Result);

            var commentResult = (commentResponse.Result as OkObjectResult).Value;
            var commentId = commentResult as string;

            var response = await imagesController.DeleteComment(commentId);
            Assert.IsInstanceOf<NoContentResult>(response);

            var comment = await imagesController.GetComment(commentId);

            Assert.IsInstanceOf<BadRequestObjectResult>(comment.Result);
            Assert.AreEqual("Caff comment does not exists.", (comment.Result as BadRequestObjectResult).Value);
            await CleanUp(caffDto.Id);
        }

        [Test]
        public async Task SearchByCaption_NoResult()
        {
            await UploadCaff();
            var search = await imagesController.SearchCaffsByCaption(new SearchByCaptionDto { Query = "notexistingquerycaption" });
            Assert.IsInstanceOf<OkObjectResult>(search.Result);
            var list = (search.Result as OkObjectResult).Value as List<CaffDto>;
            Assert.IsEmpty(list);
        }

        [Test]
        public async Task SearchByCaption_HasResult()
        {
            var caffDto = await UploadCaff();
            var search = await imagesController.SearchCaffsByCaption(new SearchByCaptionDto { Query = "scenery" });
            Assert.IsInstanceOf<OkObjectResult>(search.Result);
            var list = (search.Result as OkObjectResult).Value as List<CaffDto>;
            Assert.IsNotEmpty(list);
            Assert.IsTrue(CaffDtoEquals(caffDto, list.Single()));
        }

        [Test]
        public async Task SearchByCaption_Null()
        {
            var caffDto = await UploadCaff();
            var search = await imagesController.SearchCaffsByCaption(null);
            Assert.IsInstanceOf<OkObjectResult>(search.Result);
            var list = (search.Result as OkObjectResult).Value as List<CaffDto>;
            Assert.IsNotEmpty(list);
            Assert.IsTrue(CaffDtoEquals(caffDto, list.Single()));
        }

        [Test]
        public async Task SearchByCaption_EmptyString()
        {
            var caffDto = await UploadCaff();
            var search = await imagesController.SearchCaffsByCaption(new SearchByCaptionDto { Query = "" });
            Assert.IsInstanceOf<OkObjectResult>(search.Result);
            var list = (search.Result as OkObjectResult).Value as List<CaffDto>;
            Assert.IsNotEmpty(list);
            Assert.IsTrue(CaffDtoEquals(caffDto, list.Single()));
        }

        [Test]
        public async Task SearchByTags_NoResult()
        {
            await UploadCaff();
            var search = await imagesController.SearchCaffsByTags(new SearchByTagsDto { QueryTags = new List<string> { "asd1", "asd2" } });
            Assert.IsInstanceOf<OkObjectResult>(search.Result);
            var list = (search.Result as OkObjectResult).Value as List<CaffDto>;
            Assert.IsEmpty(list);
        }

        [Test]
        public async Task SearchByTags_HasResult()
        {
            var caffDto = await UploadCaff();
            var search = await imagesController.SearchCaffsByTags(new SearchByTagsDto { QueryTags = new List<string> { "mountains" } });
            Assert.IsInstanceOf<OkObjectResult>(search.Result);
            var list = (search.Result as OkObjectResult).Value as List<CaffDto>;
            Assert.IsNotEmpty(list);
            Assert.IsTrue(CaffDtoEquals(caffDto, list.Single()));
        }

        [Test]
        public async Task SearchByTags_Null()
        {
            var caffDto = await UploadCaff();
            var search = await imagesController.SearchCaffsByTags(null);
            Assert.IsInstanceOf<OkObjectResult>(search.Result);
            var list = (search.Result as OkObjectResult).Value as List<CaffDto>;
            Assert.IsNotEmpty(list);
            Assert.IsTrue(CaffDtoEquals(caffDto, list.Single()));
        }

        [Test]
        public async Task SearchByTags_EmptyList()
        {
            var caffDto = await UploadCaff();
            var search = await imagesController.SearchCaffsByTags(new SearchByTagsDto { QueryTags = new List<string>() });
            Assert.IsInstanceOf<OkObjectResult>(search.Result);
            var list = (search.Result as OkObjectResult).Value as List<CaffDto>;
            Assert.IsNotEmpty(list);
            Assert.IsTrue(CaffDtoEquals(caffDto, list.Single()));
        }

        private async Task<CaffDto> UploadCaff()
        {
            using var stream = new MemoryStream(File.ReadAllBytes(@"TestData\1.caff").ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "1.caff", "1.caff");
            var uploadResponse = await imagesController.UploadCaff(formFile);
            Assert.IsInstanceOf<OkObjectResult>(uploadResponse.Result);

            var caff = (uploadResponse.Result as OkObjectResult).Value;
            return caff as CaffDto;
        }

        private async Task CleanUp(string caffId) => await imagesController.DeleteCaff(caffId);

        private static bool CaffDtoEquals(CaffDto x, CaffDto y)
        {
            if (!CaffEqualsWithoutSequenceProperties(x, y))
                return false;

            return x.Ciffs.ToList().SequenceEqual(y.Ciffs.ToList(), CiffDtoEquals) &&
                   x.Comments.ToList().SequenceEqual(y.Comments.ToList(), CaffCommentReturnDtoEquals) &&
                   x.Captions.ToList().SequenceEqual(y.Captions.ToList()) &&
                   x.Tags.ToList().SequenceEqual(y.Tags.ToList());
        }

        private static bool CaffEqualsWithoutSequenceProperties(CaffDto x, CaffDto y)
            => EqualityComparerHelper.PropertiesEqual(x, y, nameof(CaffDto.Ciffs), nameof(CaffDto.Captions), nameof(CaffDto.Comments), nameof(CaffDto.Tags));

        private static bool CiffDtoEquals(CiffDto x, CiffDto y)
            => EqualityComparerHelper.PropertiesEqual(x, y);

        private static bool CaffCommentReturnDtoEquals(CaffCommentReturnDto x, CaffCommentReturnDto y)
        {
            return x.Comment == y.Comment && x.Id == y.Id && x.CaffId == y.CaffId && x.UserId == y.UserId;
        }

        private static bool CaffCommentEquals(CaffComment x, CaffComment y)
        {
            if (!CaffCommentEqualsWithoutUserAndCaff(x, y))
                return false;

            return x.Caff.Id == y.Caff.Id && x.User.Id == y.User.Id;
        }

        private static bool CaffCommentEqualsWithoutUserAndCaff(CaffComment x, CaffComment y)
            => EqualityComparerHelper.PropertiesEqual(x, y, nameof(CaffComment.User), nameof(CaffComment.Caff));
    }
}