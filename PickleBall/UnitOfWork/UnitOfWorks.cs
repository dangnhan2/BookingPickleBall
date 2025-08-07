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
            Payment = new PaymentRepo(_context);
            CourtTimeSlot = new CourtTimeSlotRepo(_context);
            BookingTimeSlot = new BookingTimeSlotRepo(_context);
            RefreshToken = new RefreshTokenRepo(_context);  
        }

        public IUserRepo User { get; }

        public IBlogRepo Blog { get; }

        public ICourtRepo Court { get; }

        public IBookingRepo Booking { get; }

        public ITimeSlotRepo TimeSlot { get; }

        public IPaymentRepo Payment { get; }

        public ICourtTimeSlotRepo CourtTimeSlot { get; }

        public IBookingTimeSlotRepo BookingTimeSlot { get; }

        public IRefreshTokenRepo RefreshToken { get; }

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
