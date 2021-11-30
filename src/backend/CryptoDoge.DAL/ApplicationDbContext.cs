using CryptoDoge.Model.Entities;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;

namespace CryptoDoge.DAL
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public override DbSet<User> Users { get; set; }
        public DbSet<Caff> Caffs { get; set; }
        public DbSet<Ciff> Ciffs { get; set; }
        public DbSet<CiffTag> CiffTags { get; set; }
        public DbSet<CaffComment> CaffComments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Cascade;
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

            var adminUser = new User
            {
                Id = "bcc978f4-aeb6-435f-8fa9-09f6c97735c9",
                UserName = "Admin User",
                Email = "admin@mail.com",
                NormalizedUserName = "Admin User",
                NormalizedEmail = "admin@mail.com",
            };

            user1.PasswordHash = passwordHasher.HashPassword(user1, "Password1!");
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "AdminAdmin1!");

            builder.Entity<User>()
                .HasData(new User[]
                {
                    user1,
                    adminUser
                });

            var adminRole = new IdentityRole { Id = "59381279-94ad-4e44-9817-d0c46350464a", Name = "ADMIN", NormalizedName = "ADMIN" };
            var userRole = new IdentityRole { Id = "34790367-a250-442c-bf25-2adbcf3f39a4", Name = "USER", NormalizedName = "USER" };

            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole[]
                {
                    adminRole,
                    userRole,
                });

            builder.Entity<IdentityUserRole<string>>()
                .HasData(new IdentityUserRole<string>[]
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = userRole.Id,
                        UserId = user1.Id
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = adminRole.Id,
                        UserId = adminUser.Id
                    },
                });
        }
    }
}
