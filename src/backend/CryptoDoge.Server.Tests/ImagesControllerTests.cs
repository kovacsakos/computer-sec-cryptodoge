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
            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);

            identityService = new IdentityService(new HttpContextAccessor { HttpContext = mockHttpContext.Object }, userManager.Object);
            imagesController = new ImagesController(parserService, imagingService, identityService);

            imagesController.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object,
            };
        }

        [Test]
        public async Task GetCaffsAsync_NoResult()
        {
            var controller = new ImagesController(parserService, imagingService, identityService);
            var response = (await controller.GetCaffs());
            Assert.IsInstanceOf<OkObjectResult>(response.Result);
            Assert.IsEmpty((response.Result as OkObjectResult).Value as List<CaffDto>);
        }

        [Test]
        public async Task GetCaffByIdAsync_NoResult()
        {
            var controller = new ImagesController(parserService, imagingService, identityService);
            var response = (await controller.GetCaffByIdAsync("notexisting")).Result;
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task GetCaffComment_NotExist()
        {
            var controller = new ImagesController(parserService, imagingService, identityService);
            var response = (await controller.GetComment("id"));
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            Assert.AreEqual("Caff comment does not exists.", (response as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task DeleteCaffAsync()
        {
            var controller = new ImagesController(parserService, imagingService, identityService);
            var response = await controller.DeleteCaff("id");
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

            // CleanUp
            await imagesController.DeleteCaff((result as CaffDto).Id);
        }
    }
}