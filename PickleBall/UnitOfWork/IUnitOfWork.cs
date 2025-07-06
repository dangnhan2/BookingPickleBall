using PickleBall.Repository;

namespace PickleBall.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {   
        public IUserRepo User { get; }
        public IBlogRepo Blog { get; }
        public ICourtRepo Court { get; }
        public IBookingRepo Booking { get; }
        public Task CompleteAsync();
    }
}
