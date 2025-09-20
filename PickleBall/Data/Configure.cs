using Microsoft.EntityFrameworkCore;
using PickleBall.Models;

namespace PickleBall.Data
{
    public static class Configure
    {
        public static ModelBuilder Configuration(this ModelBuilder model) {

            model.Entity<BookingTimeSlots>()
                .HasKey(bts => new { bts.BookingId, bts.TimeSlotId });

            model.Entity<CourtTimeSlot>()
                .HasKey(cts => new {cts.CourtID, cts.TimeSlotID });

            model.Entity<CourtTimeSlot>()
                .HasOne(ctl => ctl.Court)
                .WithMany(ctl => ctl.CourtTimeSlots)
                .HasForeignKey(tl => tl.CourtID)
                .OnDelete(DeleteBehavior.Cascade);

            model.Entity<CourtTimeSlot>()
                .HasOne(ctl => ctl.TimeSlot)
                .WithMany(ctl => ctl.CourtTimeSlots)
                .HasForeignKey(tl => tl.TimeSlotID);

            model.Entity<Court>()
                //.HasQueryFilter(r => !r.IsDeleted)
                .HasMany(c => c.Bookings)
                .WithOne(c => c.Court)
                .HasForeignKey(c => c.CourtID)
                .OnDelete(DeleteBehavior.Cascade);


            //model.Entity<Booking>()
            //    .HasQueryFilter(r => !r.IsDeleted);

            //model.Entity<User>()
            //    .HasQueryFilter(u => !u.IsDeleted);

            //model.Entity<Payment>()
            //    .HasQueryFilter(p => !p.IsDeleted);

            model.Entity<Blog>()
                .HasQueryFilter(p => !p.IsDeleted)
                .HasOne(p => p.User)
                .WithMany(p => p.Blogs)
                .HasForeignKey(p => p.UserID);

            return model;
        }
    }
}
