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
        private Mock<SignInManager<User>> signInManager;
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
            using var stream = new MemoryStream(File.ReadAllBytes(@"TestData\1.caff").ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "1.caff", "1.caff");
            var uploadResponse = await imagesController.UploadCaff(formFile);
            var caff = (uploadResponse.Result as OkObjectResult).Value;
            var caffId = (caff as CaffDto).Id;

            var response = (await imagesController.GetCaffs());
            var responseResult = response.Result;
            var caffList = (responseResult as OkObjectResult).Value as List<CaffDto>;

            Assert.IsInstanceOf<OkObjectResult>(responseResult);
            Assert.IsNotEmpty(caffList);
            Assert.AreEqual(caffId, caffList.Single().Id);

            await CleanUp(caffId);
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
            using var stream = new MemoryStream(File.ReadAllBytes(@"TestData\1.caff").ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "1.caff", "1.caff");
            var uploadResponse = await imagesController.UploadCaff(formFile);
            Assert.IsInstanceOf<OkObjectResult>(uploadResponse.Result);

            var caff = (uploadResponse.Result as OkObjectResult).Value;
            var caffDto = caff as CaffDto;
            var caffId = caffDto.Id;

            var response = await imagesController.GetCaffByIdAsync(caffId);
            Assert.IsInstanceOf<OkObjectResult>(response.Result);
            var responseResult = (response.Result as OkObjectResult).Value;
            var caffResult = responseResult as CaffDto;

            Assert.IsNotNull(caffResult);
            Assert.AreEqual(caffId, caffResult.Id);

            await CleanUp(caffId);
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
            using var stream = new MemoryStream(File.ReadAllBytes(@"TestData\1.caff").ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "1.caff", "1.caff");
            var uploadResponse = await imagesController.UploadCaff(formFile);
            Assert.IsInstanceOf<OkObjectResult>(uploadResponse.Result);
            var caff = (uploadResponse.Result as OkObjectResult).Value;
            var caffDto = caff as CaffDto;
            var caffId = caffDto.Id;

            var commentResponse = await imagesController.PostComment(caffId, new CaffCommentDto { Comment = "Valami" });
            Assert.IsInstanceOf<OkObjectResult>(commentResponse.Result);
            var commentResult = (commentResponse.Result as OkObjectResult).Value;
            var commentId = commentResult as string;

            var comment = await imagesController.GetComment(commentId);
            Assert.IsInstanceOf<OkObjectResult>(comment.Result);
            var caffCommentResult = (comment.Result as OkObjectResult).Value;
            var caffComment = caffCommentResult as CaffComment;

            Assert.IsNotNull(caffComment);
            Assert.AreEqual("Valami", caffComment.Comment);
            await CleanUp(caffId);
        }

        [Test]
        public async Task UpdateCaffComment_AfterUpload()
        {
            using var stream = new MemoryStream(File.ReadAllBytes(@"TestData\1.caff").ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "1.caff", "1.caff");
            var uploadResponse = await imagesController.UploadCaff(formFile);
            Assert.IsInstanceOf<OkObjectResult>(uploadResponse.Result);

            var caff = (uploadResponse.Result as OkObjectResult).Value;
            var caffDto = caff as CaffDto;
            var caffId = caffDto.Id;

            var commentResponse = await imagesController.PostComment(caffId, new CaffCommentDto { Comment = "Valami" });
            Assert.IsInstanceOf<OkObjectResult>(commentResponse.Result);
            var commentResult = (commentResponse.Result as OkObjectResult).Value;
            var commentId = commentResult as string;

            var updateResult = await imagesController.UpdateComment(commentId, new CaffCommentDto { Comment = "Update" });
            Assert.IsInstanceOf<OkResult>(updateResult);

            var comment = await imagesController.GetComment(commentId);
            Assert.IsInstanceOf<OkObjectResult>(comment.Result);

            var caffCommentResult = (comment.Result as OkObjectResult).Value;
            var caffComment = caffCommentResult as CaffComment;

            Assert.IsNotNull(caffComment);
            Assert.AreEqual("Update", caffComment.Comment);

            await CleanUp(caffId);
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
            using var stream = new MemoryStream(File.ReadAllBytes(@"TestData\1.caff").ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "1.caff", "1.caff");
            var response = await imagesController.UploadCaff(formFile);
            Assert.IsInstanceOf<OkObjectResult>(response.Result);

            var result = (response.Result as OkObjectResult).Value;

            Assert.IsInstanceOf<CaffDto>(result);
            Assert.IsNotNull(result);

            await CleanUp((result as CaffDto).Id);
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
            using var stream = new MemoryStream(File.ReadAllBytes(@"TestData\1.caff").ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "1.caff", "1.caff");
            var uploadResponse = await imagesController.UploadCaff(formFile);
            Assert.IsInstanceOf<OkObjectResult>(uploadResponse.Result);

            var caff = (uploadResponse.Result as OkObjectResult).Value;
            var caffDto = caff as CaffDto;
            var caffId = caffDto.Id;

            var commentResponse = await imagesController.PostComment(caffId, new CaffCommentDto { Comment = "Valami" });
            Assert.IsInstanceOf<OkObjectResult>(commentResponse.Result);

            var commentResult = (commentResponse.Result as OkObjectResult).Value;
            var commentId = commentResult as string;

            Assert.IsNotNull(commentId);
            Assert.IsNotEmpty(commentId);

            await CleanUp(caffId);
        }

        private async Task CleanUp(string caffId) => await imagesController.DeleteCaff(caffId);
    }
}