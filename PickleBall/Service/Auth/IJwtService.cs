using PickleBall.Dto;
using PickleBall.Models;

namespace PickleBall.Service.Auth
{
    public interface IJwtService
    {
        public Task<ApiResponse<LoginResponse>> GenerateToken(Partner user, HttpContext context);
        public Task<ApiResponse<LoginResponse>> GenerateRefreshToken(string refreshToken, HttpContext context);
    }
}
