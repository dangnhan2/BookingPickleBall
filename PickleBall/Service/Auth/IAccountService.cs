using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.Models;

namespace PickleBall.Service.Auth
{
    public interface IAccountService
    {
        public Task<ApiResponse<LoginResponse>> Login(LoginRequest request, HttpContext context);
        public Task<ApiResponse<string>> Logout(HttpContext context);
        public Task<ApiResponse<string>> ChangePassword(string userId, ChangePasswordRequest passwordRequest, HttpContext context);
        public Task<ApiResponse<LoginResponse>> RefreshToken(HttpContext context);
        public Task<ApiResponse<string>> CreatePartnerByAdmin(RegisterPartnerRequest request);
        public Task<ApiResponse<string>> CreateAdmin(RegisterRequest request);
    }
}
