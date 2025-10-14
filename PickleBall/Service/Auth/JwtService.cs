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
        private readonly UserManager<Partner> _userManager;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;

        public JwtService(IUnitOfWorks unitOfWorks, UserManager<Partner> userManager)
        {
            Env.Load();
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("SECRET_KEY")));
            _unitOfWork = unitOfWorks;
            _userManager = userManager;
        }
        public async Task<Result<LoginResponse>> GenerateRefreshToken(string refreshToken, HttpContext context)
        {
            var isExistToken = await _unitOfWork.RefreshToken.GetAsync(refreshToken.HashRefreshToken());

            if (isExistToken == null)
                return Result<LoginResponse>.Fail("Token is invalid", StatusCodes.Status401Unauthorized);

            if (isExistToken.IsLocked == true)
                return Result<LoginResponse>.Fail("Tài khoản đã bị khóa, vui lòng đăng nhập lại", StatusCodes.Status400BadRequest);

            if (isExistToken.ExpiresAt < DateTime.UtcNow)
                return Result<LoginResponse>.Fail("Token is invalid", StatusCodes.Status401Unauthorized);

            var userToDto = await GenerateToken(isExistToken.User, context);
            _unitOfWork.RefreshToken.Delete(isExistToken);

            return Result<LoginResponse>.Ok(userToDto.Data, StatusCodes.Status200OK);
        }

        public async Task<Result<LoginResponse>> GenerateToken(Partner user, HttpContext context)
        {
            var credentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var issuer = Env.GetString("ISSUER");
            var audience = Env.GetString("AUDIENCE");

            // claims
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
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = credentials,
                Issuer = issuer,
                Audience = audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(jwt);

            string refresh = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();

            var refreshToken = new RefreshTokens
            {
                UserID = user.Id,
                RefreshToken = refresh.HashRefreshToken(),
                IsLocked = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMonths(3)
            };

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
                    Role = userRole.First(),
                },
                AccessToken = token
            };


            _unitOfWork.RefreshToken.Add(refreshToken);
            await _unitOfWork.CompleteAsync();

            context.Response.Cookies.Append(
                  "refresh_token",
                  refresh,
                  new CookieOptions
                  {
                      HttpOnly = true,
                      Secure = true,
                      SameSite = SameSiteMode.None,
                      Path = "/",
                      Expires = DateTime.UtcNow.AddMonths(3)
                  });

            return Result<LoginResponse>.Ok(loginResponse, StatusCodes.Status200OK);
        }

    }
}
