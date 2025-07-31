using PickleBall.Data;
using PickleBall.Repository;

namespace PickleBall.UnitOfWork
{
    public class UnitOfWorks : IUnitOfWorks
    {
        private readonly BookingContext _context;

        public UnitOfWorks(BookingContext context)
        {
            _context = context;
            User = new UserRepo(_context);
            Blog = new BlogRepo(_context);
            Court = new CourtRepo(_context);
            Booking = new BookingRepo(_context);
            TimeSlot = new TimeSlotRepo(_context);
            Payement = new PaymentRepo(_context);
        }

        public IUserRepo User { get; }

        public IBlogRepo Blog { get; }

        public ICourtRepo Court { get; }

        public IBookingRepo Booking { get; }

        public ITimeSlotRepo TimeSlot { get; }

        public IPaymentRepo Payement { get; }

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
