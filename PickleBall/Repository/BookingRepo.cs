using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository
{
    public interface IBookingRepo
    {
        public IQueryable<Booking> Get();
        public Task<Booking?> GetById(Guid id);
        public Task CreateAsync(Booking booking);
        public void Update(Booking booking);
        public void Delete(Booking booking);
    }

    public class BookingRepo : IBookingRepo
    {
        private readonly BookingContext _bookingContext;

        public BookingRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public async Task CreateAsync(Booking booking)
        {
           await _bookingContext.Bookings.AddAsync(booking);
        }

        public void Delete(Booking booking)
        {
            _bookingContext.Bookings.Remove(booking);
        }

        public IQueryable<Booking> Get()
        {
            return _bookingContext.Bookings.AsQueryable();
        }

        public async Task<Booking?> GetById(Guid id)
        {
            return await _bookingContext.Bookings.Include(b => b.Payments).FirstOrDefaultAsync(b => b.ID == id);
        }

        public void Update(Booking booking)
        {
            _bookingContext.Bookings.Update(booking);
        }
    }
}
