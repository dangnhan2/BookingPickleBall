using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PickleBall.Data;
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
        private readonly string avatar = "https://res.cloudinary.com/dtihvekmn/image/upload/v1751645852/istockphoto-1337144146-612x612_llpkam.jpg";

        public AccountService(UserManager<User> userManager, IEmailService email, IJwtService jwtService, IUnitOfWorks unitOfWorks)
        {
            _userManager = userManager;
            _email = email;
            _jwtService = jwtService;
            _unitOfWorks = unitOfWorks;
        }
        public async Task<Result<UserDto>> Login(LoginRequest request)
        {
            var validator = new LoginRequestValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<UserDto>.Fail(error.ErrorMessage);
                }
            }
            
            var user = await _userManager.FindByEmailAsync(request.Email);

            //Console.Write(user);

            if (user == null)
            {
               return Result<UserDto>.Fail("Thông tin đăng nhập không đúng");
            }

            var isPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            //Console.Write(isPassword);

            if (isPassword == false)
            {
               return Result<UserDto>.Fail("Thông tin đăng nhập không đúng");
            }

            if (user.Status == UserStatus.Inactive)
            {
               return Result<UserDto>.Fail("Tài khoản đã bị khóa, vui lòng liên hệ với admin");
            }


            if (!user.EmailConfirmed)
            {
               return Result<UserDto>.Fail("Tài khoản chưa được đăng kí");
            }     

            var reponse = await _jwtService.GenerateToken(user);

            return Result<UserDto>.Ok(reponse);
        }

        public async Task<Result<string>> Logout(string refreshToken)
        {
            var isTokenExist = await _unitOfWorks.RefreshToken.GetAsync(refreshToken);

            if(isTokenExist == null)
                return Result<string>.Fail("Token không hợp lệ / không tìm thấy");

            _unitOfWorks.RefreshToken.Delete(isTokenExist);
            await _unitOfWorks.CompleteAsync();

            return Result<string>.Ok("Đăng xuất thành công");
        }

        public async Task<Result<string>> Register(RegisterRequest request)
        {   
            var validator = new RegisterRequestValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                foreach(var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage);
                }
            }

            var users = _unitOfWorks.User.Get();

            if (await users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber))
                return Result<string>.Fail("Số điện thoại đã được đăng kí");

            Env.Load();
            var checkMail = await CheckEmail(request.Email);

            if (!checkMail) 
                return Result<string>.Fail("Email đã được đăng kí");

            var newUser = new User
            {
                FullName = request.FullName,
                UserName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Avatar = avatar,
                IsDeleted = false,
                IsAdmin = false,
                EmailConfirmed = false,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                CreatedAt = DateTime.UtcNow
            };

            var reponse = await _userManager.CreateAsync(newUser, request.Password);

            if (!reponse.Succeeded)
            {
                return Result<string>.Fail("Đăng kí thất bại");
            }

            await _userManager.AddToRoleAsync(newUser, "Customer");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            token = WebUtility.UrlEncode(token);

            await SendEmail("ConfirmEmail", newUser.Email, newUser.Id, token);
            
            return Result<string>.Ok("Email đã được gửi, hãy kiểm tra email để xác nhận đăng kí");

        }

        public async Task<bool> CheckEmail(string email)
        {
            var isEmailExist = await _userManager.FindByEmailAsync(email);

            if (isEmailExist == null) {
                return true;
            }

            return false;
        }

        public async Task<Result<string>> ChangePassword(string userId, ChangePasswordRequest passwordRequest)
        {   
            var validator = new ChangePasswordRequestValidator();

            var result = validator.Validate(passwordRequest);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage);
                }
            }
           
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Result<string>.Fail("Không tìm thấy người dùng");

            var reponse = await _userManager.ChangePasswordAsync(user, passwordRequest.CurrentPassword, passwordRequest.NewPassword);

            if (!reponse.Succeeded)        
                return Result<string>.Fail("Đổi mật khẩu không thành công");

            return Result<string>.Ok("Đổi mật khẩu thành công");

        }

        public async Task<Result<User>> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new KeyNotFoundException("Không tìm thấy người dùng");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebUtility.UrlEncode(token);

            await SendEmail("ForgotPassword", user.Email, null, token);

            return Result<User>.Ok(user);
        }

        public async Task<Result<string>> ResetPassword(ForgetPasswordRequest passwordRequest)
        {   
            var validator = new ForgetPasswordRequestValidator();

            var result = validator.Validate(passwordRequest);

            if (!result.IsValid)
            {
                foreach(var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage);
                }
            }

            var user = await _userManager.FindByEmailAsync(passwordRequest.Email);

            if (user == null)
                return Result<string>.Fail("Không tìm thấy người dùng");

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, passwordRequest.Token, passwordRequest.Password);

            if (!resetPasswordResult.Succeeded) 
                return Result<string>.Fail("Đổi mật khẩu không thành công");
            

            return Result<string>.Ok("Đổi mật khẩu thành công");
        }

        public async Task<Result<string>> RegisterForAdmin(RegisterRequest request)
        {
            var isExistEmail = await _userManager.FindByEmailAsync(request.Email);
            var users = _unitOfWorks.User.Get();

            if (isExistEmail != null)
            {
                return Result<string>.Fail("Email đã được đăng kí");
            }

            if (users.Any(u => u.PhoneNumber == request.PhoneNumber))
            {
                return Result<string>.Fail("Số điện thoại đã được đăng kí");
            }

            var newUser = new User
            {
                FullName = request.FullName,
                UserName = request.FullName,
                NormalizedUserName = request.FullName.ToUpper(),
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

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                return Result<string>.Fail("Đăng kí thất bại");
            }

            await _userManager.AddToRoleAsync(newUser, "Admin");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            token = WebUtility.UrlEncode(token);

            await SendEmail("ConfirmEmail", newUser.Email, newUser.Id, token);

            return Result<string>.Ok("Email đã được gửi, hãy kiểm tra email để xác nhận đăng kí");
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
    }
}
