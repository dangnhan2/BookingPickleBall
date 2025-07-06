using Microsoft.EntityFrameworkCore;
using PickleBall.Models;

namespace PickleBall.Data
{
    public static class Configure
    {
        public static ModelBuilder Configuration(this ModelBuilder model) {

            model.Entity<Booking>()
                .HasOne(b => b.Payments)
                .WithOne(b => b.Bookings)
                .HasForeignKey<Payment>(c => c.BookingID);

            model.Entity<Court>()
                .HasQueryFilter(r => !r.IsDeleted)
                .HasMany(c => c.Bookings)
                .WithOne(c => c.Courts)
                .HasForeignKey(c => c.CourtID)
                .OnDelete(DeleteBehavior.Cascade);

            model.Entity<Booking>()
                .HasQueryFilter(r => !r.IsDeleted);

            model.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);

            model.Entity<Payment>()
                .HasQueryFilter(p => !p.IsDeleted);

            model.Entity<Blog>()
                .HasQueryFilter(p => !p.IsDeleted)
                .HasOne(p => p.User)
                .WithMany(p => p.Blogs)
                .HasForeignKey(p => p.UserID);

            return model;
        }
    }
}
