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
using PickleBall.Service.SoftService;
using PickleBall.UnitOfWork;
using System.Net;

namespace PickleBall.Service
{
    public class AccountService : IAccountService
    {   
        private readonly UserManager<User> _userManager;
        private readonly BookingContext _bookingContext;
        private readonly IEmailService _email;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly string avatar = "https://res.cloudinary.com/dtihvekmn/image/upload/v1751645852/istockphoto-1337144146-612x612_llpkam.jpg";

        public AccountService(UserManager<User> userManager, BookingContext bookingContext, IEmailService email, IJwtService jwtService, IUnitOfWorks unitOfWorks)
        {
            _userManager = userManager;
            _bookingContext = bookingContext;
            _email = email;
            _jwtService = jwtService;
            _unitOfWorks = unitOfWorks;
        }
        public async Task<Result<UserDto>> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            Console.Write(user);

            if (user == null)
            {
               return Result<UserDto>.Fail("Thông tin đăng nhập không đúng");
            }

            var isPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            Console.Write(isPassword);

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

            var result = await _jwtService.GenerateToken(user);

            return Result<UserDto>.Ok(result);
        }

        public async Task<Result<string>> Logout(string refreshToken)
        {
            var isTokenExist = await _bookingContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.RefreshToken == refreshToken);

            if(isTokenExist == null)
                return Result<string>.Fail("Token không hợp lệ / không tìm thấy");

            _bookingContext.RefreshTokens.Remove(isTokenExist);
            await _bookingContext.SaveChangesAsync();

            return Result<string>.Ok("Đăng xuất thành công");
        }

        public async Task<Result<string>> Register(RegisterRequest request)
        {
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

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                return Result<string>.Fail(result.Errors.ToString());
            }

            await _userManager.AddToRoleAsync(newUser, "Customer");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            token = WebUtility.UrlEncode(token);

            string subject = "Xác nhận đăng ký";
            
            var confirmationUrl = $"{Env.GetString("BASE_URL")}/api/Email/confirm-email?userId={newUser.Id}&token={token}";

            string htmlBody = $"<p>Nhấn vào link sau để xác nhận tài khoản:</p><a href='{confirmationUrl}'>Confirm</a>";

            await _email.EmailSender(newUser.Email, subject, htmlBody);

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
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Result<string>.Fail("Không tìm thấy người dùng");

            var result = await _userManager.ChangePasswordAsync(user, passwordRequest.CurrentPassword, passwordRequest.NewPassword);

            if (!result.Succeeded)        
                return Result<string>.Fail("Đổi mật khẩu không thành công");

            return Result<string>.Ok("Đổi mật khẩu thành công");

        }

        public async Task<Result<User>> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new KeyNotFoundException("Không tìm thấy người dùng");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebUtility.UrlEncode(token);

            string subject = "Xác nhận đặt lại mật khầu";

            var confirmationUrl = $"{Env.GetString("BASE_URL")}/api/Email/confirm-resetpassword?email={email}&token={token}";

            string htmlBody = $"<p>Nhấn vào link sau để xác nhận đặt lại mật khẩu:</p><a href='{confirmationUrl}'>Confirm</a>";
            await _email.EmailSender(user.Email, subject, htmlBody);

            return Result<User>.Ok(user);
        }

        public async Task<Result<string>> ResetPassword(ForgetPasswordRequest passwordRequest)
        {
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
                return Result<string>.Fail(result.Errors.ToString());
            }

            await _userManager.AddToRoleAsync(newUser, "Admin");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            token = WebUtility.UrlEncode(token);

            string subject = "Xác nhận đăng ký";

            var confirmationUrl = $"{Env.GetString("BASE_URL")}/api/Email/confirm-email?userId={newUser.Id}&token={token}";

            string htmlBody = $"<p>Nhấn vào link sau để xác nhận tài khoản:</p><a href='{confirmationUrl}'>Confirm</a>";

            await _email.EmailSender(newUser.Email, subject, htmlBody);

            return Result<string>.Ok("Email đã được gửi, hãy kiểm tra email để xác nhận đăng kí");
        }

    }
}
