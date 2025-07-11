using CloudinaryDotNet;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PickleBall.Data;
using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.Models;
using PickleBall.Service.SoftService;
using PickleBall.UnitOfWork;
using System.Net;
using System.Text;

namespace PickleBall.Service
{
    public class AccountService : IAccountService
    {   
        private readonly UserManager<User> _userManager;
        private readonly BookingContext _bookingContext;
        private readonly IEmailService _email;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWorks _unitOfWorks;

        public AccountService(UserManager<User> userManager, BookingContext bookingContext, IEmailService email, IJwtService jwtService, IUnitOfWorks unitOfWorks)
        {
            _userManager = userManager;
            _bookingContext = bookingContext;
            _email = email;
            _jwtService = jwtService;
            _unitOfWorks = unitOfWorks;
        }
        public async Task<UserDto> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new InvalidDataException("Thông tin đăng nhập không đúng"); 
            }

            var isPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            Console.Write(isPassword);

            if (isPassword == false)
            {
                throw new ArgumentException("Thông tin đăng nhập không đúng");
            }

            var result = await _jwtService.GenerateToken(user);

            return result;
        }

        public async Task Logout(string refreshToken)
        {
            var isTokenExist = await _bookingContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.RefreshToken == refreshToken)
                ?? throw new KeyNotFoundException("Token is invalid");

            _bookingContext.RefreshTokens.Remove(isTokenExist);
            await _bookingContext.SaveChangesAsync();
        }

        public async Task Register(RegisterRequest request)
        {
            var users = _unitOfWorks.User.Get();

            if(await users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber))
            {
                throw new ArgumentException("Số điện thoại đã được đăng kí");
            }

            Env.Load();
            var checkMail = await CheckEmail(request.Email);

            if (checkMail == false) {
                throw new ArgumentException("Email đã được đăng ký");
            }

            var newUser = new User
            {
                FullName = request.FullName,
                UserName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Avatar = "https://res.cloudinary.com/dtihvekmn/image/upload/v1751645852/istockphoto-1337144146-612x612_llpkam.jpg",
                IsDeleted = false,
                EmailConfirmed = false,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                throw new Exception("Không thể khởi tạo tài khoản");
            }

            await _userManager.AddToRoleAsync(newUser, "Customer");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            token = WebUtility.UrlEncode(token);

            string subject = "Xác nhận đăng ký";
            
            var confirmationUrl = $"{Env.GetString("BASE_URL")}/api/Email/confirm-email?userId={newUser.Id}&token={token}";

            string htmlBody = $"<p>Nhấn vào link sau để xác nhận tài khoản:</p><a href='{confirmationUrl}'>Confirm</a>";

            await _email.EmailSender(newUser.Email, subject, htmlBody);

        }

        public async Task<bool> CheckEmail(string email)
        {
            var isEmailExist = await _userManager.FindByEmailAsync(email);

            if (isEmailExist == null) {
                return true;
            }

            return false;
        }

    }
}
