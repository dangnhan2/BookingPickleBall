using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository.RefreshToken
{
    public interface IRefreshTokenRepo
    {
        public Task<IEnumerable<RefreshTokens>> GetExpiredRefreshToken();
        public Task<RefreshTokens?> GetAsync(string token);
        public void Add(RefreshTokens tokens);
        public void Delete(RefreshTokens refreshTokens);
        public void RemoveRefreshTokens(IEnumerable<RefreshTokens> tokens);
    }

    public class RefreshTokenRepo : IRefreshTokenRepo
    {
        private readonly BookingContext _bookingContext;

        public RefreshTokenRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public void Add(RefreshTokens tokens)
        {
            _bookingContext.RefreshTokens.Add(tokens);
        }

        public void Delete(RefreshTokens refreshTokens)
        {
            _bookingContext.RefreshTokens.Remove(refreshTokens);
        }

        public async Task<RefreshTokens?> GetAsync(string token)
        {
            return await _bookingContext.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(r => r.RefreshToken == token);
        }

        public async Task<IEnumerable<RefreshTokens>> GetExpiredRefreshToken()
        {
            return await _bookingContext.RefreshTokens.Where(t => t.ExpiresAt < DateTime.UtcNow).ToListAsync();
        }

        public void RemoveRefreshTokens(IEnumerable<RefreshTokens> tokens)
        {
            _bookingContext.RefreshTokens.RemoveRange(tokens);
        }
    }

}
