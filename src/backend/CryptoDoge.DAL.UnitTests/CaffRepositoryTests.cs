using CryptoDoge.DAL.Repositories;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoDoge.DAL.UnitTests
{
    public class CaffRepositoryTests
    {
		private DbContextOptions<ApplicationDbContext> options;
		private ICaffRepository caffRepository;
		private ApplicationDbContext dbContext;
		private User botUser;
		private User user;
		private static int id = 1;
		private List<Caff> testCaffs;

		private const string first = "c4586be7-dfc4-4cbf-8d64-992e1843c600";
		private const string second = "b2b306b5-364d-48c6-bab8-7b1bc49998c5";
		private const string third = "7779f7c0-7bb0-4efd-a058-54dfd57460bb";

		[SetUp]
		public async Task SetupAsync()
		{
			options = new DbContextOptionsBuilder<ApplicationDbContext>()
							.UseInMemoryDatabase(databaseName: "CryptoDogeTestDb")
							.Options;

			dbContext = new ApplicationDbContext(options);
			await dbContext.Database.EnsureDeletedAsync();

			caffRepository = new CaffRepository(dbContext);
			botUser = new User { Id = "c193a1f7-0000-0000-0000-976f4811bf5f", UserName = "Approve Bot" };
			user = new User { Id = "c193a1f7-0000-1111-0000-976f4811bf5f", UserName = "User" };

			testCaffs = CreateTestCaffs(first, second, third);
		}

		[Test]
		public async Task AddNewCaffAsync_Test()
        {
			await caffRepository.AddNewCaffAsync(testCaffs[0]);
			var dbCaff = await dbContext.Caffs.FindAsync(first);
			Assert.IsNotNull(dbCaff);
			Assert.IsTrue(CaffEquals(testCaffs[0], dbCaff));
		}

		[Test]
		public async Task GetCaffsAsync_NoResult()
        {
			var dbCaffs = await caffRepository.GetCaffsAsync();
			Assert.IsEmpty(dbCaffs);
		}

		[Test]
		public async Task GetCaffsAsync_HasResult()
		{
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			var dbCaffs = await caffRepository.GetCaffsAsync();
			Assert.IsNotEmpty(dbCaffs);
			Assert.AreEqual(testCaffs.Count, dbCaffs.Count());
			Assert.IsTrue(dbCaffs.ToList().SequenceEqual(testCaffs, CaffEquals));
		}

		[Test]
		public async Task GetCaffByIdAsync_NoResult()
        {
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			var dbCaff = await caffRepository.GetCaffByIdAsync("1d66077c-9684-456c-a1db-df9dc08683fe");
			Assert.IsNull(dbCaff);
		}

		[Test]
		public async Task GetCaffByIdAsync_HasResult()
        {
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			var dbCaff = await caffRepository.GetCaffByIdAsync(second);
			Assert.IsNotNull(dbCaff);
			Assert.IsTrue(CaffEquals(testCaffs[1], dbCaff));
		}

		[Test]
		public async Task DeleteCaffAsync()
        {
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			await caffRepository.DeleteCaffAsync(testCaffs[1]);
			var dbCaff = await caffRepository.GetCaffByIdAsync(second);
			Assert.IsNull(dbCaff);
			Assert.AreEqual(2, dbContext.Caffs.Count());
		}

		[Test]
		public async Task AddCaffCommentAsync()
        {
			var id = "9e8c760c-4c7d-4a95-b462-da40f1a6c36e";
			var comment = new CaffComment
			{
				Id = id,
				Caff = testCaffs[0],
				User = user,
				Comment = "Beautiful!",
			};
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			await caffRepository.AddCaffCommentAsync(comment);

			var dbComment = await dbContext.CaffComments.FindAsync(id);
			Assert.IsNotNull(dbComment);
			Assert.IsTrue(CaffCommentEquals(comment, dbComment));
		}

		[Test]
		public async Task GetCaffCommentByIdAsync_NoResult()
        {
			var id = "9e8c760c-4c7d-4a95-b462-da40f1a6c36e";
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			var dbComment = await caffRepository.GetCaffCommentByIdAsync(id);
			Assert.IsNull(dbComment);
		}

		[Test]
		public async Task GetCaffCommentByIdAsync_Successful()
		{
			var id = "9e8c760c-4c7d-4a95-b462-da40f1a6c36e";
			var comment = new CaffComment
			{
				Id = id,
				Caff = testCaffs[0],
				User = user,
				Comment = "Beautiful!",
			};
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			await caffRepository.AddCaffCommentAsync(comment);

			var dbComment = await caffRepository.GetCaffCommentByIdAsync(id);
			Assert.IsNotNull(dbComment);
			Assert.IsTrue(CaffCommentEquals(comment, dbComment));
		}

		[Test]
		public async Task UpdateCaffCommentAsync()
        {
			var id = "9e8c760c-4c7d-4a95-b462-da40f1a6c36e";
			var comment = new CaffComment
			{
				Id = id,
				Caff = testCaffs[0],
				User = user,
				Comment = "Beautiful!",
			};
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			await caffRepository.AddCaffCommentAsync(comment);

			comment.Comment = "Beautiful landscape";
			await caffRepository.UpdateCaffCommentAsync(comment);
			var dbComment = await caffRepository.GetCaffCommentByIdAsync(id);
			Assert.IsNotNull(dbComment);
			Assert.IsTrue(CaffCommentEquals(comment, dbComment));
		}

		[Test]
		public async Task DeleteCaffCommentAsync()
        {
			var id = "9e8c760c-4c7d-4a95-b462-da40f1a6c36e";
			var comment = new CaffComment
			{
				Id = id,
				Caff = testCaffs[0],
				User = user,
				Comment = "Beautiful!",
			};
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			await caffRepository.AddCaffCommentAsync(comment);
			await caffRepository.DeleteCaffCommentAsync(comment);

			var dbComment = await caffRepository.GetCaffCommentByIdAsync(id);
			Assert.IsNull(dbComment);
		}

		[Test]
		public async Task SearchByCaption_NoResult()
        {
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			var result = await caffRepository.SearchCaffsByCaption("asdasdasd");
			Assert.IsEmpty(result);
		}

		[Test]
		public async Task SearchByCaption_HasResult()
        {
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			var result = await caffRepository.SearchCaffsByCaption("Nature");
			Assert.IsNotEmpty(result);
			Assert.AreEqual(3, result.Count());
		}

		[Test]
		public async Task SearchByTags_NoResult()
		{
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			var result = await caffRepository.SearchCaffsByTags(new List<string> { "asdsad", "fdgfdgdfg" });
			Assert.IsEmpty(result);
		}

		[Test]
		public async Task SearchByTags_HasResult()
		{
			testCaffs.ForEach(async testCaff => await caffRepository.AddNewCaffAsync(testCaff));
			var result = await caffRepository.SearchCaffsByTags(new List<string> { "nature", "beautiful" });
			Assert.IsNotEmpty(result);
			Assert.AreEqual(3, result.Count());
		}


		private static bool CaffEquals(Caff x, Caff y)
		{
			if (!CaffEqualsWithoutCiffsAndComments(x, y))
				return false;

			return x.Ciffs.ToList().SequenceEqual(y.Ciffs.ToList(), CiffEquals) &&
				x.Comments.ToList().SequenceEqual(y.Comments.ToList(), CaffCommentEquals);
		}

		private static bool CaffEqualsWithoutCiffsAndComments(Caff x, Caff y)
			=> EqualityComparerHelper.PropertiesEqual(x, y, nameof(Caff.Ciffs), nameof(Caff.Comments));

		private static bool CiffEquals(Ciff x, Ciff y)
		{
			if (!CiffEqualsWithoutTags(x, y))
				return false;

			return x.Tags.SequenceEqual(y.Tags);
		}
		private static bool CiffEqualsWithoutTags(Ciff x, Ciff y)
			=> EqualityComparerHelper.PropertiesEqual(x, y, nameof(Ciff.Tags));

		private static bool CaffCommentEquals(CaffComment x, CaffComment y)
        {
			if (!CaffCommentEqualsWithoutUserAndCaff(x, y))
				return false;

			return x.Caff.Id == y.Caff.Id && x.User.Id == y.User.Id;
		}

        private static bool CaffCommentEqualsWithoutUserAndCaff(CaffComment x, CaffComment y) 
			=> EqualityComparerHelper.PropertiesEqual(x, y, nameof(CaffComment.User), nameof(CaffComment.Caff));

        private List<Caff> CreateTestCaffs(string first, string second, string third) => new List<Caff>
		{
			CreateCaff(first, new string[] {"Albania", "Australia", "Brazil", "Denmark", "England"} ),
			CreateCaff(second, new string[] { "Haiti", "Iceland", "Indonesia", "Italy", "Jamaica" } ),
			CreateCaff(third, new string[] { "Mexico", "Montenegro", "Luxembourg", "Nepal", "Hungary"} ),
		};

        private Caff CreateCaff(string guid, string[] countries)
        {
			var caff = new Caff
			{
				Id = guid,
				NumberOfAnimations = 3,
				UploadedBy = botUser,
				CreationYear = 2021, CreationMonth = 11, CreationDay = 28,
				CreationHour = 10, CreationMinute = 10,
				Creator = "Creative Creator",
				Ciffs = new List<Ciff>(),
				Comments = new List<CaffComment>(),
			};

			var ciffs = CreateCiffs(caff, countries);
			var comments = CreateComments(caff, new List<string> { "Approved." });

			ciffs.ForEach(ciff => caff.Ciffs.Add(ciff));
			comments.ForEach(ciff => caff.Comments.Add(ciff));
			return caff;
		}

        private static List<Ciff> CreateCiffs(Caff caff, string[] countries) => new()
        {
            CreateCiff(caff, $"Nature in {countries[0]}", new List<string> { "nature", $"{countries[0]}", "landscape", "beautiful", "mountains"} ),
            CreateCiff(caff, $"Balance in {countries[1]}", new List<string> { "france", $"{countries[1]}", "bigcity" } ),
            CreateCiff(caff, $"Sunset in {countries[2]}", new List<string> { $"{countries[2]}", "sunset", "beautiful" } ),
            CreateCiff(caff, $"Mountains in {countries[3]}", new List<string> { $"{countries[3]}", "skiing", "mountains"} ),
            CreateCiff(caff, $"Trip in {countries[4]}", new List<string> { "bridges", "beautiful", "bigcity", "mycountry", $"{countries[4]}" }),
        };

        private static Ciff CreateCiff(Caff caff, string caption, List<string> tags) => new()
        {
			Id = Guid.NewGuid().ToString(),
			Caff = caff,
			Caption = caption,
			Tags = CreateTags(tags),
		};

        private static List<CiffTag> CreateTags(List<string> tags) => tags.Select(x => new CiffTag { Id = id++, Value = x }).ToList();

        private List<CaffComment> CreateComments(Caff caff, List<string> comments) 
			=> comments
                .Select(comment => new CaffComment
                {
                    Id = Guid.NewGuid().ToString(),
                    Caff = caff,
                    Comment = comment,
                    User = botUser,
                })
                .ToList();
    }
}
