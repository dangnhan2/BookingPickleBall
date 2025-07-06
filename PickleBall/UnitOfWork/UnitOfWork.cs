using PickleBall.Data;
using PickleBall.Repository;

namespace PickleBall.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookingContext _context;

        public UnitOfWork(BookingContext context)
        {
            _context = context;
            User = new UserRepo(_context);
            Blog = new BlogRepo(_context);
            Court = new CourtRepo(_context);
            Booking = new BookingRepo(_context);
        }

        public IUserRepo User { get; }

        public IBlogRepo Blog { get; }

        public ICourtRepo Court { get; }

        public IBookingRepo Booking { get; }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
