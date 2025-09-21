using PickleBall.Dto;
using PickleBall.Models;

namespace PickleBall.Service.Auth
{
    public interface IJwtService
    {
        public Task<TokenResponse> GenerateToken(User user);
        public Task<Result<TokenResponse>> GenerateRefreshToken(string refreshToken);
    }
}
