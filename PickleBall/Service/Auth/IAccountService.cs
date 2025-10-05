using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.Models;

namespace PickleBall.Service.Auth
{
    public interface IAccountService
    {
        public Task<Result<LoginResponse>> Login(LoginRequest request, HttpContext context);
        public Task<Result<string>> Logout(HttpContext context);
        public Task<Result<string>> ChangePassword(string userId, ChangePasswordRequest passwordRequest, HttpContext context);
        public Task<Result<LoginResponse>> RefreshToken(HttpContext context);
        public Task<Result<string>> CreatePartnerByAdmin(RegisterPartnerRequest request);
        public Task<Result<string>> CreateAdmin(RegisterRequest request);
    }
}
