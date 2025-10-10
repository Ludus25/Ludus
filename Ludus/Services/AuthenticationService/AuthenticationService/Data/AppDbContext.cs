using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AuthenticationService.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;

namespace AuthenticationService.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
                
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
           // SeedAdminUser(builder);

        }

        /* private static void SeedAdminUser(ModelBuilder builder)
         {

             var admin = new User
             {
                 Id = "1",
                 UserName = "ludusadm1@gmail.com",
                 NormalizedUserName = "LUDUSADM1@GMAIL.COM",
                 Email = "ludusadm1@gmail.com",
                 NormalizedEmail = "LUDUSADM1@GMAIL.COM",
                 EmailConfirmed = true,
                 SecurityStamp = "STATIC-SECURITY-STAMP-0001",
                 ConcurrencyStamp = "STATIC-CONCURRENCY-STAMP-0001",
                 name = "Admin",
                 surname = "Admin",
                 mlb = "1203998710890",
                 PasswordHash = "$2y$10$IWHM/QhahGfo86siqBCFrOeqtqjafIFoT6RG3vaiPdgnuJWnESStW",
                 TwoFactorEnabled = true
             };


             builder.Entity<User>().HasData(admin);
             builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
             {
                 RoleId = "cc10e60a-8052-4227-8a8c-d1c22e01480e", 
                 UserId = "1"
             });


         }*/
        private static void SeedAdminUser(ModelBuilder builder)
        {
            // Seed role
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "cc10e60a-8052-4227-8a8c-d1c22e01480e",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "c3506bfd-f2ce-4170-884b-2c065d2d98e8",
                    Name = "User",
                    NormalizedName = "USER"
                }
            );

            // Seed admin user
            var admin = new User
            {
                Id = "1",
                UserName = "ludusadm1@gmail.com",
                NormalizedUserName = "LUDUSADM1@GMAIL.COM",
                Email = "ludusadm1@gmail.com",
                NormalizedEmail = "LUDUSADM1@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = "STATIC-SECURITY-STAMP-0001",
                ConcurrencyStamp = "STATIC-CONCURRENCY-STAMP-0001",
                name = "Admin",
                surname = "Admin",
                mlb = "1203998710890",
                PasswordHash = "$2y$10$IWHM/QhahGfo86siqBCFrOeqtqjafIFoT6RG3vaiPdgnuJWnESStW",
                TwoFactorEnabled = true
            };

            builder.Entity<User>().HasData(admin);

            // Link user ↔ role
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = "cc10e60a-8052-4227-8a8c-d1c22e01480e",
                UserId = "1"
            });
        }





    }

}

