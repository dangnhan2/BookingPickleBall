using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.Models;

namespace PickleBall.Service.Auth
{
    public interface IAccountService
    {
        public Task<Result<string>> Login(LoginRequest request, HttpContext context);
        public Task<Result<string>> Register(RegisterRequest request);
        public Task<Result<string>> RegisterForAdmin(RegisterRequest request);
        public Task<Result<string>> Logout(HttpContext context);
        public Task<Result<string>> ChangePassword(string userId, ChangePasswordRequest passwordRequest);
        public Task<Result<User>> ForgotPassword(string email);
        public Task<Result<string>> ResetPassword(ForgetPasswordRequest passwordRequest);
        public Task<Result<string>> RefreshToken(HttpContext context);
    }
}
