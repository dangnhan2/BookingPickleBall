using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository.Bookings
{
    public interface IBookingRepo
    {
        public IQueryable<Booking> Get();
        public Task<IEnumerable<Booking>> GetExpiredBookings();
        public Task<Booking?> GetById(Guid id);
        public Task<Booking?> GetByOrderCode(string orderCode);
        public void Create(Booking booking);
        public void Update(Booking booking);
        public void Delete(Booking booking);
        public void DeleteExpiredBookingTimeSlot(IEnumerable<BookingTimeSlots> bookings);
    }

    public class BookingRepo : IBookingRepo
    {
        private readonly BookingContext _bookingContext;

        public BookingRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public void Create(Booking booking)
        {
           _bookingContext.Bookings.Add(booking);
        }

        public void Delete(Booking booking)
        {
            _bookingContext.Bookings.Remove(booking);
        }

        public void DeleteExpiredBookings(IEnumerable<Booking> bookings)
        {
            _bookingContext.Bookings.RemoveRange(bookings);
        }

        public void DeleteExpiredBookingTimeSlot(IEnumerable<BookingTimeSlots> bookings)
        {
            _bookingContext.BookingTimeSlots.RemoveRange(bookings);
        }

        public IQueryable<Booking> Get()
        {
            return _bookingContext.Bookings.AsQueryable();
        }

        public async Task<Booking?> GetById(Guid id)
        {
            return await _bookingContext.Bookings.Include(b => b.User).FirstOrDefaultAsync(b => b.ID == id);
        }

        public async Task<Booking?> GetByOrderCode(string orderCode)
        {
            return await _bookingContext.Bookings
                .Include(b => b.BookingTimeSlots)
                .ThenInclude(s => s.TimeSlot)
                .FirstOrDefaultAsync(b => b.TransactionId == orderCode);
        }

        public async Task<IEnumerable<Booking>> GetExpiredBookings()
        {
            return await _bookingContext.Bookings
                .Include(b => b.BookingTimeSlots)
                   .ThenInclude(s => s.TimeSlot)
                .Where(b => b.BookingStatus == Models.Enum.BookingStatus.Pending && b.ExpriedAt < DateTime.UtcNow)
                .ToListAsync();
        }

        public void Update(Booking booking)
        {
            _bookingContext.Bookings.Update(booking);
        }
    }
}
