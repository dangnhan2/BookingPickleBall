using Microsoft.EntityFrameworkCore;
using PickleBall.Models;
using System.Reflection.Emit;

namespace PickleBall.Data
{
    public static class Configure
    {
        public static ModelBuilder Configuration(this ModelBuilder model) {

            model.Entity<BookingTimeSlots>()
                .HasKey(bts => new { bts.BookingId, bts.CourtTimeSlotId });

            model.Entity<CourtTimeSlot>()
                .HasKey(cts => new {cts.CourtID, cts.TimeSlotID });

            model.Entity<CourtTimeSlot>()
                 .HasKey(cts => cts.ID);

            model.Entity<CourtTimeSlot>()
                .HasOne(ctl => ctl.Court)
                .WithMany(ctl => ctl.CourtTimeSlots)
                .HasForeignKey(tl => tl.CourtID)
                .OnDelete(DeleteBehavior.Cascade);

            model.Entity<CourtTimeSlot>()
                .HasOne(ctl => ctl.TimeSlot)
                .WithMany(ctl => ctl.CourtTimeSlots)
                .HasForeignKey(tl => tl.TimeSlotID);

            model.Entity<BookingTimeSlots>()
                .HasOne(bts => bts.Booking)
                .WithMany(bts => bts.BookingTimeSlots)
                .HasForeignKey(bts => bts.BookingId);

            model.Entity<BookingTimeSlots>()
                .HasOne(bts => bts.CourtTimeSlots)
                .WithMany(bts => bts.BookingTimeSlots)
                .HasForeignKey(bts => bts.CourtTimeSlotId);

            model.Entity<Court>()
                //.HasQueryFilter(r => !r.IsDeleted)
                .HasMany(c => c.Bookings)
                .WithOne(c => c.Court)
                .HasForeignKey(c => c.CourtID)
                .OnDelete(DeleteBehavior.Cascade);
            
            model.Entity<Partner>()
                .HasMany(p => p.Courts)
                .WithOne(p => p.Partner)
                .HasForeignKey(p => p.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            model.Entity<Partner>()
                .HasMany(p => p.TimeSlots)
                .WithOne(p => p.Partner)
                .HasForeignKey(p => p.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            model.Entity<Blog>()
                .HasQueryFilter(p => !p.IsDeleted)
                .HasOne(p => p.Partner)
                .WithMany(p => p.Blogs)
                .HasForeignKey(p => p.PartnerID);

            return model;
        }
    }
}
