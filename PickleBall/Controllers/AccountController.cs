using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service;

namespace PickleBall.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IJwtService _jwtService;

        public AccountController(IAccountService accountService, IJwtService jwtService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var result = await _accountService.Login(request);

                Response.Cookies.Append(
                    "refresh_token",
                    result.RefreshToken,

                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Path = "/",
                        Expires = DateTime.UtcNow.AddMinutes(15)
                    });

                return Ok(new
                {
                    Message = "Đăng nhập thành công",
                    StatusCode = StatusCodes.Status200OK,
                    AccessToken = result.AccessToken
                });
               
            }catch(InvalidDataException ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,   
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                await _accountService.Register(request);

                return Ok(new
                {
                    Message = "Một đường dẫn đã gửi đến email của bạn, hãy nhấn xác nhận",
                    StatusCode = StatusCodes.Status200OK,
                });
            }
            catch (ArgumentException ex) {
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            try
            {
                await _accountService.Logout(refreshToken);

                Response.Cookies.Append(
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

                return Ok(new
                {
                    Message = "Đăng xuất thành công",
                    StatusCode = StatusCodes.Status200OK,
                });
            }catch(KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status404NotFound,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            try
            {
                var result = await _jwtService.GenerateRefreshToken(refreshToken);

                Response.Cookies.Append(
                   "refresh_token",
                   result.RefreshToken,

                   new CookieOptions
                   {
                       HttpOnly = true,
                       Secure = true,
                       SameSite = SameSiteMode.None,
                       Path = "/",
                       Expires = DateTime.UtcNow.AddMinutes(15)
                   });

                return Ok(new
                {
                    Message = "Refresh token succeed",
                    StatusCode = StatusCodes.Status200OK,
                    AccessToken = result.AccessToken
                });
            }catch(ArgumentException ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }
    }
}
