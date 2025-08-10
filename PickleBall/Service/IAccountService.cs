
using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.Models;

namespace PickleBall.Service
{
    public interface IAccountService
    {
        public Task<Result<UserDto>> Login(LoginRequest request);
        public Task<Result<string>> Register(RegisterRequest request);
        public Task<Result<string>> RegisterForAdmin(RegisterRequest request);
        public Task<Result<string>> Logout(string refreshToken);
        public Task<Result<string>> ChangePassword(string userId, ChangePasswordRequest passwordRequest);
        public Task<Result<User>> ForgotPassword(string email);
        public Task<Result<string>> ResetPassword(ForgetPasswordRequest passwordRequest);
    }
}
