using DotNetEnv;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.Extension;
using PickleBall.Models;
using PickleBall.Models.Enum;
using PickleBall.Service.Email;
using PickleBall.UnitOfWork;
using PickleBall.Validation;
using System.Net;


namespace PickleBall.Service.Auth
{
    public class AccountService : IAccountService
    {   
        private readonly UserManager<Partner> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IBackgroundJobClient _backgroundJob;
        private const string avatar = "https://res.cloudinary.com/dtihvekmn/image/upload/v1751645852/istockphoto-1337144146-612x612_llpkam.jpg";

        public AccountService(UserManager<Partner> userManager, IJwtService jwtService, IUnitOfWorks unitOfWorks, IBackgroundJobClient backgroundJob)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _unitOfWorks = unitOfWorks;
            _backgroundJob = backgroundJob;
        }
        public async Task<Result<LoginResponse>> Login(LoginRequest request, HttpContext context)
        {
            var validator = new LoginRequestValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<LoginResponse>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }
            
            var user = await _userManager.FindByEmailAsync(request.Email);
           
            var isPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if (user == null || !isPassword)           
               return Result<LoginResponse>.Fail("Thông tin đăng nhập không đúng", StatusCodes.Status400BadRequest);         
 
            var reponse = await _jwtService.GenerateToken(user);

            var roles = await _userManager.GetRolesAsync(user);

            var loginResponse = new LoginResponse
            {
                Data = new UserDto
                {
                    ID = user.Id,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    BussinessName = user.BussinessName,
                    Avatar = user.Avatar,
                    IsApproved = user.IsApproved,
                    Role = roles.First(),
                },
                AccessToken = reponse.AccessToken
            };

            context.Response.Cookies.Append(
                   "refresh_token",
                   reponse.RefreshToken,

                   new CookieOptions
                   {
                       HttpOnly = true,
                       Secure = true,
                       SameSite = SameSiteMode.None,
                       Path = "/",
                       Expires = DateTime.UtcNow.AddMonths(1)
                   });

            return Result<LoginResponse>.Ok(loginResponse, StatusCodes.Status200OK);
        }

        public async Task<Result<string>> Logout(HttpContext context)
        {
            var refreshToken = context.Request.Cookies["refresh_token"];

            var isTokenExist = await _unitOfWorks.RefreshToken.GetAsync(refreshToken.HashRefreshToken());

            if(isTokenExist == null)
                return Result<string>.Fail("Token không hợp lệ / không tìm thấy", StatusCodes.Status401Unauthorized);

            _unitOfWorks.RefreshToken.Delete(isTokenExist);
            await _unitOfWorks.CompleteAsync();

            context.Response.Cookies.Append(
                    "refresh_token",
                    string.Empty,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Path = "/",
                        Expires = DateTimeOffset.UnixEpoch,
                    });


            return Result<string>.Ok("Đăng xuất thành công", StatusCodes.Status200OK);
        }

        public async Task<Result<string>> ChangePassword(string userId, ChangePasswordRequest passwordRequest, HttpContext context)
        {
            var refreshToken = context.Request.Cookies["refresh_token"];

            var isExistToken = await _unitOfWorks.RefreshToken.GetAsync(refreshToken.HashRefreshToken());

            if (isExistToken == null)
                return Result<string>.Fail("Token không hợp/không tìm thấy", StatusCodes.Status401Unauthorized);

            if (isExistToken.IsLocked == true)
                return Result<string>.Fail("Tài khoản đã bị khóa, vui lòng đăng nhập lại", StatusCodes.Status400BadRequest);

            if (isExistToken.ExpiresAt < DateTime.UtcNow)
                return Result<string>.Fail("Token đã hết hạn, vui lòng đăng nhập lại", StatusCodes.Status401Unauthorized);

           
            var validator = new ChangePasswordRequestValidator();

            var result = validator.Validate(passwordRequest);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }
           
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Result<string>.Fail("Không tìm thấy người dùng", StatusCodes.Status404NotFound);

            var reponse = await _userManager.ChangePasswordAsync(user, passwordRequest.CurrentPassword, passwordRequest.NewPassword);

            if (!reponse.Succeeded)        
                return Result<string>.Fail("Đổi mật khẩu không thành công", StatusCodes.Status400BadRequest);

            _unitOfWorks.RefreshToken.Delete(isExistToken);
            await _unitOfWorks.CompleteAsync();

            context.Response.Cookies.Append(
                   "refresh_token",
                   string.Empty,
                   new CookieOptions
                   {
                       HttpOnly = true,
                       Secure = true,
                       SameSite = SameSiteMode.None,
                       Path = "/",
                       Expires = DateTimeOffset.UnixEpoch,
                   });

            return Result<string>.Ok("Đổi mật khẩu thành công", StatusCodes.Status200OK );

        }

        public async Task<bool> CheckEmail(string email)
        {
            var isEmailExist = await _userManager.FindByEmailAsync(email);

            if (isEmailExist == null)
            {
                return true;
            }

            return false;
        }

        public async Task<Result<string>> RefreshToken(HttpContext context)
        {
            var refreshToken = context.Request.Cookies["refresh_token"];

            var isExistToken = await _unitOfWorks.RefreshToken.GetAsync(refreshToken.HashRefreshToken());

            if (isExistToken == null)
                return Result<string>.Fail("Token không hợp/không tìm thấy", StatusCodes.Status401Unauthorized);

            if (isExistToken.IsLocked == true)
                return Result<string>.Fail("Tài khoản đã bị khóa, vui lòng đăng nhập lại", StatusCodes.Status400BadRequest);

            if (isExistToken.ExpiresAt < DateTime.UtcNow)
                return Result<string>.Fail("Token đã hết hạn, vui lòng đăng nhập lại", StatusCodes.Status401Unauthorized);

            var userToDto = await _jwtService.GenerateToken(isExistToken.User);

            context.Response.Cookies.Append(
                  "refresh_token",
                  userToDto.RefreshToken,

                  new CookieOptions
                  {
                      HttpOnly = true,
                      Secure = true,
                      SameSite = SameSiteMode.None,
                      Path = "/",
                      Expires = DateTime.UtcNow.AddMonths(1)
                  });

            return Result<string>.Ok(userToDto.AccessToken, StatusCodes.Status200OK);
        }

        public async Task<Result<string>> CreatePartnerByAdmin(RegisterPartnerRequest request)
        {
           
                var isEmailExist = await _userManager.FindByEmailAsync(request.Email);

                if (isEmailExist != null)
                {
                    return Result<string>.Fail("Email đã được đăng kí", StatusCodes.Status400BadRequest);
                }



                var partner = new Partner
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    UserName = request.FullName,
                    NormalizedEmail = request.Email.ToUpper(),
                    BussinessName = request.BussinessName,
                    PhoneNumber = request.PhoneNumber,
                    Avatar = avatar,
                    EmailConfirmed = true,
                    IsApproved = true,
                    IsAdmin = false
                };

                var result = await _userManager.CreateAsync(partner, "123456");

                if (!result.Succeeded)
                {
                    return Result<string>.Fail("Đăng kí thất bại", StatusCodes.Status400BadRequest);
                }
                await _userManager.AddToRoleAsync(partner, "Partner");

                return Result<string>.Ok("Đăng kí thành công", StatusCodes.Status201Created);
            
        }
    }
}
