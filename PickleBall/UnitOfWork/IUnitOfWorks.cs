using PickleBall.Repository;

namespace PickleBall.UnitOfWork
{
    public interface IUnitOfWorks : IDisposable
    {   
        public IUserRepo User { get; }
        public IBlogRepo Blog { get; }
        public ICourtRepo Court { get; }
        public IBookingRepo Booking { get; }
        public ITimeSlotRepo TimeSlot { get; }
        public ICourtTimeSlotRepo CourtTimeSlot { get; }
        public IBookingTimeSlotRepo BookingTimeSlot { get; }
        public IRefreshTokenRepo RefreshToken { get; }
        public Task CompleteAsync();
    }
}
