using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PickleBall.Data;
using PickleBall.Dto;
using PickleBall.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace PickleBall.Service
{
    public class JwtService : IJwtService
    {
        private readonly BookingContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;

        public JwtService(BookingContext context, UserManager<User> userManager)
        {
            Env.Load();
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("SECRET_KEY")));
            _context = context;
            _userManager = userManager;
        }
        public async Task<UserDto> GenerateRefreshToken(string refreshToken)
        {
            var isExistToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.RefreshToken == refreshToken) ?? throw new KeyNotFoundException("Không tìm thấy token");

            if(isExistToken.IsLocked == true)
            {
                throw new ArgumentException("Tài khoản đã bị khóa, vui lòng liên hệ với quản trị viên");
            }

            if(isExistToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new ArgumentException("Token đã hết hạn, hãy đăng nhập lại");
            }

            var userToDto = await GenerateToken(isExistToken.User);

            var newRefreshToken = new RefreshTokens
            {
                UserID = isExistToken.UserID,
                RefreshToken = userToDto.RefreshToken,
                IsLocked = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };

            _context.RefreshTokens.Remove(isExistToken);

            await  _context.RefreshTokens.AddAsync(newRefreshToken);

            await _context.SaveChangesAsync();

            return userToDto;
        }

        public async Task<UserDto> GenerateToken(User user)
        {
            var credentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var issuer = Env.GetString("ISSUER");
            var audience = Env.GetString("AUDIENCE");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRole = await _userManager.GetRolesAsync(user);
            claims.AddRange(userRole.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = credentials,
                Issuer = issuer,
                Audience = audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(jwt);

            var refreshToken = new RefreshTokens
            {
                UserID = user.Id,
                RefreshToken = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString(),
                IsLocked = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            var userToDto = new UserDto
            {
                AccessToken = token,
                RefreshToken = refreshToken.RefreshToken,
            };

            return userToDto;
        }
    }
}
