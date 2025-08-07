using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PickleBall.Models;

namespace PickleBall.Data
{
    public static class SeedData
    {
        public static ModelBuilder Seed(this ModelBuilder model)
        {
            model.Entity<IdentityRole>()
                .HasData(
                    new IdentityRole
                    {
                        Id = "e6081ef2-337b-436d-b1cc-c66cb49203c1",
                        Name = "Customer",
                        NormalizedName = "Customer".ToUpper()
                    },
                    new IdentityRole
                    {
                        Id = "afcab8c4-ba4f-4331-83c1-80d44c2c8e78",
                        Name = "Admin",
                        NormalizedName = "Admin".ToUpper(),
                    }
                );

            //model.Entity<TimeSlot>()
            //    .HasData(
            //        new TimeSlot
            //        {
            //            ID = Guid.NewGuid(),
            //            StartTime = new TimeOnly(8, 0),
            //            EndTime = new TimeOnly(10, 0)
            //        },
            //         new TimeSlot
            //         {
            //             ID = Guid.NewGuid(),
            //             StartTime = new TimeOnly(10, 0),
            //             EndTime = new TimeOnly(12, 0)
            //         },
            //         new TimeSlot
            //         {
            //             ID = Guid.NewGuid(),
            //             StartTime = new TimeOnly(12, 0),
            //             EndTime = new TimeOnly(14, 0)
            //         },
            //         new TimeSlot
            //         {
            //             ID = Guid.NewGuid(),
            //             StartTime = new TimeOnly(14, 0),
            //             EndTime = new TimeOnly(16, 0)
            //         },
            //         new TimeSlot
            //         {
            //             ID = Guid.NewGuid(),
            //             StartTime = new TimeOnly(16, 0),
            //             EndTime = new TimeOnly(18, 0)
            //         },
            //         new TimeSlot
            //         {
            //             ID = Guid.NewGuid(),
            //             StartTime = new TimeOnly(18, 0),
            //             EndTime = new TimeOnly(20, 0)
            //         },
            //          new TimeSlot
            //          {
            //              ID = Guid.NewGuid(),
            //              StartTime = new TimeOnly(20, 0),
            //              EndTime = new TimeOnly(22, 0)
            //          }
            //    );

            return model;
        }
    }
}
