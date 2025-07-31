using PickleBall.Dto;
using PickleBall.Models;

namespace PickleBall.Service
{
    public interface IJwtService
    {
        public Task<UserDto> GenerateToken(User user);
        public Task<Result<UserDto>> GenerateRefreshToken(string refreshToken);
    }
}
