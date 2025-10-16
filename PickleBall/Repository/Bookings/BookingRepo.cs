using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository.Bookings
{
    public interface IBookingRepo
    {
        public IQueryable<Booking> GetAllByPartner(Guid id);
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

        public void DeleteExpiredBookingTimeSlot(IEnumerable<BookingTimeSlots> bookings)
        {
            _bookingContext.BookingTimeSlots.RemoveRange(bookings);
        }

        public  IQueryable<Booking> GetAllByPartner(Guid id)
        {
            return  _bookingContext.Bookings  
                .Include(b => b.BookingTimeSlots)
                .Where(b => b.Court.PartnerId == id).AsQueryable();
        }

        public async Task<Booking?> GetById(Guid id)
        {
            return await _bookingContext.Bookings
                .Include(b => b.BookingTimeSlots)
                   .ThenInclude(bts => bts.CourtTimeSlots)
                   .ThenInclude(cts => cts.TimeSlot)
                .FirstOrDefaultAsync(b => b.ID == id);
        }

        public async Task<Booking?> GetByOrderCode(string orderCode)
        {
            return await _bookingContext.Bookings               
                .FirstOrDefaultAsync(b => b.TransactionId == orderCode);
        }

        public async Task<IEnumerable<Booking>> GetExpiredBookings()
        {
            return await _bookingContext.Bookings
                .Include(b => b.BookingTimeSlots)
                   //.ThenInclude(s => s.TimeSlot)
                .Where(b => b.BookingStatus == Models.Enum.BookingStatus.Pending && b.ExpriedAt < DateTime.UtcNow)
                .ToListAsync();
        }

        public void Update(Booking booking)
        {
            _bookingContext.Bookings.Update(booking);
        }
    }
}
