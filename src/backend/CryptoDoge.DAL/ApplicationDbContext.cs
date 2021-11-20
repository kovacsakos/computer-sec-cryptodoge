﻿using CryptoDoge.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CryptoDoge.DAL
{
	public class ApplicationDbContext: IdentityDbContext<User>
	{

		public override DbSet<User> Users { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }

            SeedData(builder);
        }

        private static void SeedData(ModelBuilder builder)
		{
            var passwordHasher = new PasswordHasher<User>();

            var user1 = new User
            {
                Id = "52251c06-58a7-4fe4-885d-2a484034326d",
                UserName = "First User",
                Email = "user@mail.com",
                NormalizedUserName = "First User",
                NormalizedEmail = "user@mail.com",
            };

            user1.PasswordHash = passwordHasher.HashPassword(user1, "Password");


            builder.Entity<User>()
                .HasData(new User[]
                {
                    user1
                });
        }
    }
}
