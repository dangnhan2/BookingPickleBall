using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PickleBall.Models;

namespace PickleBall.Data
{
    public class BookingContext : IdentityDbContext<User>
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options) { }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Court> Courts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.AddModel();
        }
    }
}
