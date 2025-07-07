using PickleBall.Dto;
using PickleBall.Dto.Request;

namespace PickleBall.Service
{
    public interface IAccountService
    {
        public Task<UserDto> Login(LoginRequest request);
        public Task Register(RegisterRequest request);
        public Task Logout(string refreshToken);
    }
}
