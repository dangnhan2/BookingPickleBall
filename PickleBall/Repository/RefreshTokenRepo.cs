using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository
{
    public interface IRefreshTokenRepo
    {
        public IQueryable<RefreshTokens> Get();
        public Task<RefreshTokens?> GetAsync(string token);
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

        public async Task<RefreshTokens?> GetAsync(string token)
        {
            return await _bookingContext.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(r => r.RefreshToken == token);
        }
    }

}
