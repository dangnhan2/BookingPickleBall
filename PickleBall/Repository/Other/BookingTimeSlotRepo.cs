using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository.Other
{
    public interface IBookingTimeSlotRepo
    {
        public IQueryable<Guid> FindBookedSlot(Guid courtId, DateOnly date);
        public IQueryable<BookingTimeSlots> Get();
    }

    public class BookingTimeSlotRepo : IBookingTimeSlotRepo
    {
        private readonly BookingContext _bookingContext;

        public BookingTimeSlotRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public IQueryable<Guid> FindBookedSlot(Guid courtId, DateOnly date)
        {
            return _bookingContext.BookingTimeSlots
                .Include(bts => bts.Booking)
                .Where(bts => bts.Booking.CourtID == courtId && bts.Booking.BookingDate == date).Select(tl => tl.TimeSlotId);
        }

        public IQueryable<BookingTimeSlots> Get()
        {
            return _bookingContext.BookingTimeSlots.AsQueryable();
        }
    }
}
