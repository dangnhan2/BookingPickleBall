using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository.Other
{
    public interface IBookingTimeSlotRepo
    {
        public IQueryable<BookingTimeSlots> Get();
    }

    public class BookingTimeSlotRepo : IBookingTimeSlotRepo
    {
        private readonly BookingContext _bookingContext;

        public BookingTimeSlotRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public IQueryable<BookingTimeSlots> Get()
        {
            return _bookingContext.BookingTimeSlots.AsQueryable();
        }
    }
}
