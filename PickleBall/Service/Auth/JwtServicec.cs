using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PickleBall.Data;
using PickleBall.Dto;
using PickleBall.Extension;
using PickleBall.Models;
using PickleBall.UnitOfWork;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PickleBall.Service.Auth
{
    public class JwtService : IJwtService
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;

        public JwtService(IUnitOfWorks unitOfWorks, UserManager<User> userManager)
        {
            Env.Load();
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("SECRET_KEY")));
            _unitOfWork = unitOfWorks;
            _userManager = userManager;
        }
        public async Task<Result<TokenResponse>> GenerateRefreshToken(string refreshToken)
        {
            var isExistToken = await _unitOfWork.RefreshToken.GetAsync(refreshToken.HashRefreshToken());

            if (isExistToken == null)
                return Result<TokenResponse>.Fail("Token không hợp/không tìm thấy", StatusCodes.Status401Unauthorized);

            if (isExistToken.IsLocked == true)
                return Result<TokenResponse>.Fail("Tài khoản đã bị khóa, vui lòng đăng nhập lại", StatusCodes.Status400BadRequest);

            if (isExistToken.ExpiresAt < DateTime.UtcNow)
                return Result<TokenResponse>.Fail("Token đã hết hạn, vui lòng đăng nhập lại", StatusCodes.Status401Unauthorized);

            var userToDto = await GenerateToken(isExistToken.User);

            return Result<TokenResponse>.Ok(userToDto, StatusCodes.Status200OK);
        }

        public async Task<TokenResponse> GenerateToken(User user)
        {
            var credentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var issuer = Env.GetString("ISSUER");
            var audience = Env.GetString("AUDIENCE");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRole = await _userManager.GetRolesAsync(user);
            claims.AddRange(userRole.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = credentials,
                Issuer = issuer,
                Audience = audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(jwt);

            string refresh = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();

            var userToDto = new TokenResponse
            {
                AccessToken = token,
                RefreshToken = refresh,
            };

            var refreshToken = new RefreshTokens
            {
                UserID = user.Id,
                RefreshToken = refresh.HashRefreshToken(),
                IsLocked = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMonths(7)
            };

            _unitOfWork.RefreshToken.Add(refreshToken);
            await _unitOfWork.CompleteAsync();

            return userToDto;
        }

    }
}
