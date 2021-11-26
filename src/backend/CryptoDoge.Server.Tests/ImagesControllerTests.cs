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

            var _contextAccessor = new Mock<IHttpContextAccessor>();
            var _userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            signInManager = new Mock<SignInManager<User>>(userManager.Object,
                           _contextAccessor.Object, _userPrincipalFactory.Object, null, null, null);
            identityService = new IdentityService(_contextAccessor.Object, userManager.Object);
        }

        [Test]
        public async Task GetEmptyCaffs()
        {
            var controller = new ImagesController(parserService, imagingService, identityService);
            var response = (await controller.GetCaffs()).Result as OkObjectResult;
            Assert.IsEmpty(response.Value as List<CaffDto>);
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
        public async Task DeleteCaff()
        {
            var controller = new ImagesController(parserService, imagingService, identityService);
            var response = (await controller.DeleteCaff("id"));
            Assert.IsInstanceOf<NoContentResult>(response);
        }
    }
}