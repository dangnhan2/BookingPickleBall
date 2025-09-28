using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PickleBall.Models;
using System.Runtime.InteropServices;

namespace PickleBall.Data
{
    public static class SeedData
    {
        public static ModelBuilder Seed(this ModelBuilder model)
        {
            model.Entity<IdentityRole<Guid>>()
                .HasData(
                    new IdentityRole<Guid>
                    {
                        Id = Guid.Parse("89aa6827-4e1e-4d25-8a54-d16bb532768c"),
                        Name = "Partner",
                        NormalizedName = "PARTNER".ToUpper()
                    },
                    new IdentityRole<Guid>
                    {
                        Id = Guid.Parse("be39c154-0a91-4fdc-b47e-1be4f4e7f685"),
                        Name = "Admin",
                        NormalizedName = "ADMIN".ToUpper(),
                    }
                );

            //model.Entity<IdentityUser<Guid>>().HasData(
            //    new IdentityUser<Guid>
            //    {
            //        Id = Guid.Parse("7b7247e1-89b0-4a88-aaaa-a2fa6950e1f6"),
            //        UserName = "admin",
            //        NormalizedUserName = "ADMIN",
            //        PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, "123456"),
            //        Email = "admin@gmail.com",
            //        NormalizedEmail = "ADMIN@GMAIL.COM"
            //    });

            //model.Entity<IdentityUserRole<Guid>>().HasData(
            //    new IdentityUserRole<Guid>
            //    {
            //        RoleId = Guid.Parse("be39c154-0a91-4fdc-b47e-1be4f4e7f685"),
            //        UserId = Guid.Parse("7b7247e1-89b0-4a88-aaaa-a2fa6950e1f6")

            //    });

            return model;
        }
    }
}
