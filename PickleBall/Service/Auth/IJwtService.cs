using PickleBall.Dto;
using PickleBall.Models;

namespace PickleBall.Service.Auth
{
    public interface IJwtService
    {
        public Task<Result<LoginResponse>> GenerateToken(Partner user, HttpContext context);
        public Task<Result<LoginResponse>> GenerateRefreshToken(string refreshToken, HttpContext context);
    }
}
