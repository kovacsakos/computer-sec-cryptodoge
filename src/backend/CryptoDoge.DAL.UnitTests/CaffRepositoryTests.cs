using CryptoDoge.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.DAL.UnitTests
{
    public class CaffRepositoryTests
    {
		private DbContextOptions<ApplicationDbContext> options;
		private Mock<UserManager<User>> userManager;

		[SetUp]
		public void Setup()
		{
			options = new DbContextOptionsBuilder<ApplicationDbContext>()
							.UseInMemoryDatabase(databaseName: "CryptoDogeTestDb")
							.Options;

			var _store = new Mock<IUserStore<User>>();
			userManager = new Mock<UserManager<User>>(_store.Object, null, null, null, null, null, null, null, null);
		}
	}
}
