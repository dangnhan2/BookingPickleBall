using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository
{
    public interface IRefreshTokenRepo
    {
        public IQueryable<RefreshTokens> Get();
        public void Delete(RefreshTokens refreshTokens);
    }

    public class RefreshTokenRepo : IRefreshTokenRepo
    {
        private readonly BookingContext _bookingContext;

        public RefreshTokenRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public void Delete(RefreshTokens refreshTokens)
        {
            _bookingContext.RefreshTokens.Remove(refreshTokens);
        }

        public IQueryable<RefreshTokens> Get()
        {
            return _bookingContext.RefreshTokens.AsQueryable();
        }
    }

}
