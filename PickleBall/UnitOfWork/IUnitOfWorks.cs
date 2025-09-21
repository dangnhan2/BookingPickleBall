using PickleBall.Repository.Blogs;
using PickleBall.Repository.Bookings;
using PickleBall.Repository.Courts;
using PickleBall.Repository.Other;
using PickleBall.Repository.RefreshToken;
using PickleBall.Repository.TimeSlots;
using PickleBall.Repository.Users;

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
