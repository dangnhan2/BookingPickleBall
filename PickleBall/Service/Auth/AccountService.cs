using DotNetEnv;
using FluentEmail.Core;
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
using static QRCoder.PayloadGenerator;


namespace PickleBall.Service.Auth
{
    public class AccountService : IAccountService
    {   
        private readonly UserManager<Partner> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IBackgroundJobClient _backgroundJob;
        private const string avatar = "https://res.cloudinary.com/dtihvekmn/image/upload/v1751645852/istockphoto-1337144146-612x612_llpkam.jpg";
        private readonly IEmailService _emailService;

        public AccountService(UserManager<Partner> userManager, IJwtService jwtService, IUnitOfWorks unitOfWorks, IBackgroundJobClient backgroundJob, IEmailService emailService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _unitOfWorks = unitOfWorks;
            _backgroundJob = backgroundJob;
            _emailService = emailService;
        }
        public async Task<Result<LoginResponse>> Login(LoginRequest request, HttpContext context)
        {
            var result = await new LoginRequestValidator().ValidateAsync(request);
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
 
            var reponse = await _jwtService.GenerateToken(user, context);

            return Result<LoginResponse>.Ok(reponse.Data, StatusCodes.Status200OK);
        }
        public async Task<Result<string>> Logout(HttpContext context)
        {
            var refreshToken = context.Request.Cookies["refresh_token"];
            if (refreshToken == null)
                return Result<string>.Fail("Token is invalid", StatusCodes.Status401Unauthorized);

            var isTokenExist = await _unitOfWorks.RefreshToken.GetAsync(refreshToken.HashRefreshToken());

            if(isTokenExist == null)
                return Result<string>.Fail("Token is invalid", StatusCodes.Status401Unauthorized);

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
            if (refreshToken == null)
                return Result<string>.Fail("Token is invalid", StatusCodes.Status401Unauthorized);

            if (isExistToken == null)
                return Result<string>.Fail("Token không hợp/không tìm thấy", StatusCodes.Status401Unauthorized);

            if (isExistToken.IsLocked == true)
                return Result<string>.Fail("Tài khoản đã bị khóa, vui lòng đăng nhập lại", StatusCodes.Status400BadRequest);

            if (isExistToken.ExpiresAt < DateTime.UtcNow)
                return Result<string>.Fail("Token is invalid", StatusCodes.Status401Unauthorized);

           
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
        public async Task<Result<LoginResponse>> RefreshToken(HttpContext context)
        {
            var refreshToken = context.Request.Cookies["refresh_token"];

            var response = await _jwtService.GenerateRefreshToken(refreshToken, context);

            return Result<LoginResponse>.Ok(response.Data, StatusCodes.Status200OK);
        }
        public async Task<Result<string>> CreatePartnerByAdmin(RegisterPartnerRequest request)
        {           
                var isEmailExist = await _userManager.FindByEmailAsync(request.Email);

                if (isEmailExist != null)                
                    return Result<string>.Fail("Email đã được đăng kí", StatusCodes.Status400BadRequest);
                
                var partner = new Partner
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    UserName = request.FullName,
                    NormalizedEmail = request.Email.ToUpper(),
                    BussinessName = request.BussinessName,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                    PayOSClientId = request.PayOSClientId,
                    PayOSCheckSumKey = request.PayOSCheckSumKey,
                    PayOSApiKey = request.PayOSApiKey,
                    Avatar = avatar,
                    EmailConfirmed = true,
                    IsApproved = true,
                    IsAdmin = false
                };

                var result = await _userManager.CreateAsync(partner, "123456");

                if (!result.Succeeded)                
                    return Result<string>.Fail("Đăng kí thất bại", StatusCodes.Status400BadRequest);
                
                await _userManager.AddToRoleAsync(partner, "Partner");

               

                await _emailService.EmailSender(partner.Email, "Thông tin đăng nhập", Template(partner.FullName, partner.Email, "123456"));

                return Result<string>.Ok("Đăng kí thành công", StatusCodes.Status201Created);
            
        }
        public async Task<Result<string>> CreateAdmin(RegisterRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user != null)
                return Result<string>.Fail("Email đã được đăng kí", StatusCodes.Status400BadRequest);

            var admin = new Partner
            {
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                FullName = "admin",
                UserName = request.Username,
                EmailConfirmed = true,
                IsApproved = true,
                IsAdmin = true,
                Avatar = avatar
            };

            var result = await _userManager.CreateAsync(admin, request.Password);

            if (!result.Succeeded)
            {
                return Result<string>.Fail("Đăng kí thất bại", StatusCodes.Status400BadRequest);
            }

            await _userManager.AddToRoleAsync(admin, "Admin");
            return Result<string>.Ok("Đăng kí thành công", StatusCodes.Status201Created);
        }
        private string Template(string fullName, string email, string password)
        {
            string body = $@"
                  <!DOCTYPE html>
                      <html lang='vi'>
                      <head>
                        <meta charset='UTF-8'>
                        <title>Thông tin đăng nhập</title>
                      </head>
                         <body style='font-family: Arial, sans-serif; background-color: #f7f7f7; padding: 20px;'>
                            <div style='max-width: 600px; margin: auto; background: #fff; border-radius: 8px; padding: 20px;'>
                            <h2 style='color: #4CAF50;'>Xin chào {fullName},</h2>
                            <p>Cảm ơn bạn đã đăng ký tài khoản. Dưới đây là thông tin đăng nhập của bạn:</p>

                            <table style='width: 100%; border-collapse: collapse; margin-top: 10px;'>
            
                                  <tr>
                                   <td style='padding: 8px; border: 1px solid #ddd;'><strong>Email:</strong></td>
                                   <td style='padding: 8px; border: 1px solid #ddd;'>{email}</td>
                                  </tr>
                                  <tr>
                                    <td style='padding: 8px; border: 1px solid #ddd;'><strong>Mật khẩu tạm thời:</strong></td>
                                    <td style='padding: 8px; border: 1px solid #ddd;'>{password}</td>
                                  </tr>
                              </table>

                                <p style='margin-top: 20px;'>
                                 Hãy đăng nhập ngay và đổi mật khẩu mới để bảo mật tài khoản của bạn.
                                </p>                               
                             <p style='margin-top: 30px;'>Trân trọng</p>
                           </div>
                         </body>
                      </html>";

            return body;
        }
    }
}