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
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _email;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IBackgroundJobClient _backgroundJob;
        private const string avatar = "https://res.cloudinary.com/dtihvekmn/image/upload/v1751645852/istockphoto-1337144146-612x612_llpkam.jpg";

        public AccountService(UserManager<User> userManager, IEmailService email, IJwtService jwtService, IUnitOfWorks unitOfWorks, IBackgroundJobClient backgroundJob)
        {
            _userManager = userManager;
            _email = email;
            _jwtService = jwtService;
            _unitOfWorks = unitOfWorks;
            _backgroundJob = backgroundJob;
        }
        public async Task<Result<string>> Login(LoginRequest request, HttpContext context)
        {
            var validator = new LoginRequestValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }
            
            var user = await _userManager.FindByEmailAsync(request.Email);

            //Console.Write(user);
            var isPassword = await _userManager.CheckPasswordAsync(user, request.Password);


            if (user == null && !isPassword)
            {
               return Result<string>.Fail("Thông tin đăng nhập không đúng", StatusCodes.Status400BadRequest);
            }

            //Console.Write(isPassword);

            if (user.Status == UserStatus.Inactive)
            {
               return Result<string>.Fail("Tài khoản đã bị khóa, vui lòng liên hệ với admin", StatusCodes.Status400BadRequest);
            }


            if (!user.EmailConfirmed)
            {
               return Result<string>.Fail("Tài khoản chưa được đăng kí", StatusCodes.Status400BadRequest);
            }     

            var reponse = await _jwtService.GenerateToken(user);

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

            return Result<string>.Ok(reponse.AccessToken, StatusCodes.Status200OK);
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

        public async Task<Result<string>> Register(RegisterRequest request)
        {   
            var validator = new RegisterRequestValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                foreach(var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            var users = _unitOfWorks.User.Get();

            if (await users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber))
                return Result<string>.Fail("Số điện thoại đã được đăng kí", StatusCodes.Status400BadRequest);

            Env.Load();
            var checkMail = await CheckEmail(request.Email);

            if (!checkMail) 
                return Result<string>.Fail("Email đã được đăng kí", StatusCodes.Status400BadRequest);

            var newUser = new User
            {
                FullName = request.FullName,
                UserName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Avatar = avatar,
                IsDeleted = false,
                IsAdmin = false,
                EmailConfirmed = true,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                CreatedAt = DateTime.UtcNow
            };

            var reponse = await _userManager.CreateAsync(newUser, request.Password);

            if (!reponse.Succeeded)
            {
                return Result<string>.Fail("Đăng kí thất bại", StatusCodes.Status400BadRequest);
            }

            await _userManager.AddToRoleAsync(newUser, "Customer");

            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            //token = WebUtility.UrlEncode(token);

            //_backgroundJob.Enqueue(() => SendEmail("ConfirmEmail", newUser.Email, newUser.Id, token));
            
            
            return Result<string>.Ok("Đăng kí thành công", StatusCodes.Status201Created);

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

        public async Task<Result<User>> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user == null)
            {
                return Result<User>.Fail("Không tìm thấy người dùng", StatusCodes.Status404NotFound);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebUtility.UrlEncode(token);

            _backgroundJob.Enqueue(() => SendEmail("ForgotPassword", user.Email, null, token));

            return Result<User>.Ok(user, StatusCodes.Status200OK);
        }

        public async Task<Result<string>> ResetPassword(ForgetPasswordRequest passwordRequest)
        {   
            var validator = new ForgetPasswordRequestValidator();

            var result = validator.Validate(passwordRequest);

            if (!result.IsValid)
            {
                foreach(var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            var user = await _userManager.FindByEmailAsync(passwordRequest.Email);

            if (user == null)
                return Result<string>.Fail("Không tìm thấy người dùng", StatusCodes.Status404NotFound);

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, passwordRequest.Token, passwordRequest.Password);

            if (!resetPasswordResult.Succeeded) 
                return Result<string>.Fail("Đổi mật khẩu không thành công", StatusCodes.Status400BadRequest);
            

            return Result<string>.Ok("Đổi mật khẩu thành công", StatusCodes.Status200OK);
        }

        public async Task<Result<string>> RegisterForAdmin(RegisterRequest request)
        {
            var validator = new RegisterRequestValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            var isExistEmail = await _userManager.FindByEmailAsync(request.Email);
            var users = _unitOfWorks.User.Get();

            if (isExistEmail != null)
            {
                return Result<string>.Fail("Email đã được đăng kí", StatusCodes.Status400BadRequest);
            }

            if (users.Any(u => u.PhoneNumber == request.PhoneNumber))
            {
                return Result<string>.Fail("Số điện thoại đã được đăng kí", StatusCodes.Status400BadRequest);
            }

            var newUser = new User
            {
                FullName = request.FullName,
                UserName = request.FullName,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                PhoneNumber = request.PhoneNumber,
                Avatar = avatar,
                Email = request.Email,
                IsDeleted = false,
                IsAdmin = true,
                NormalizedEmail = request.Email.ToUpper(),
                EmailConfirmed = false,
            };

            var response = await _userManager.CreateAsync(newUser, request.Password);

            if (!response.Succeeded)
            {
                return Result<string>.Fail($"Đăng kí thất bại: {response.Errors}", StatusCodes.Status400BadRequest);
            }

            await _userManager.AddToRoleAsync(newUser, "Admin");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            token = WebUtility.UrlEncode(token);

            await SendEmail("ConfirmEmail", newUser.Email, newUser.Id, token);

            return Result<string>.Ok("Email đã được gửi, hãy kiểm tra email để xác nhận đăng kí" , StatusCodes.Status200OK);
        }

        public async Task SendEmail(string type, string email, string? id, string token)
        {   
            if(type == "ConfirmEmail")
            {
                string subject = "Xác nhận đăng ký";

                var confirmationUrl = $"{Env.GetString("BASE_URL")}/api/Email/confirm-email?userId={id}&token={token}";

                string htmlBody = $"<p>Nhấn vào link sau để xác nhận tài khoản:</p><a href='{confirmationUrl}'>Confirm</a>";

                await _email.EmailSender(email, subject, htmlBody);
            }

            if(type == "ForgotPassword")
            {
                string subject = "Xác nhận đặt lại mật khầu";

                var confirmationUrl = $"{Env.GetString("BASE_URL")}/api/Email/confirm-resetpassword?email={email}&token={token}";

                string htmlBody = $"<p>Nhấn vào link sau để xác nhận đặt lại mật khẩu:</p><a href='{confirmationUrl}'>Confirm</a>";
                await _email.EmailSender(email, subject, htmlBody);
            }
            
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
    }
}
