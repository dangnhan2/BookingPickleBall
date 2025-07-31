using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                       Message = result.Error,
                       StatusCode = StatusCodes.Status400BadRequest
                    });
                }

                Response.Cookies.Append(
                    "refresh_token",
                    result.Data.RefreshToken,

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
                    AccessToken = result.Data.AccessToken,
                });

            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác: {ex.Message}");
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
                var result =  await _accountService.Register(request);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }

                return Ok(new
                {
                    Message = result.Data,
                    StatusCode = StatusCodes.Status200OK,
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác : {ex.Message}");
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(string userId, ChangePasswordRequest passwordRequest)
        {
            try
            {
                var result = await _accountService.ChangePassword(userId, passwordRequest);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }
                return Ok(new
                {
                    Message = result.Data,
                    StatusCode = StatusCodes.Status200OK,
                });
            }catch (Exception ex)
            {   
                Log.Error($"Lỗi khác: {ex.Message}");
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
                var result = await _accountService.Logout(refreshToken);

                if (!result.Success)
                {
                    Log.Error(result.Error);
                    return Unauthorized(new
                    {
                        Message = result.Error,
                        StatusCode = StatusCodes.Status401Unauthorized
                    });
                }

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
                    Message = result.Data,
                    StatusCode = StatusCodes.Status200OK,
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác: {ex.Message}");
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [Authorize]               
        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            try
            {
                var result = await _jwtService.GenerateRefreshToken(refreshToken);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }

                Response.Cookies.Append(
                   "refresh_token",
                   result.Data.AccessToken,

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
                    AccessToken = result.Data.AccessToken,
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


        [AllowAnonymous]    
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ForgetPasswordRequest passwordRequest)
        {
            try
            {
               var result =  await _accountService.ResetPassword(passwordRequest);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Message =result.Error,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }
                return Ok(new
                {
                    Message = result.Data,
                    StatusCodes = StatusCodes.Status200OK
                });
            }catch(Exception ex)
            {
                Log.Error($"Lỗi khác: {ex.Message}");
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("forgot-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {   
            try
            {
                var user = await _accountService.ForgotPassword(email);

                return Ok(new
                {
                    Message = "Email đã được gửi, hãy kiểm tra email để đặt lại mật khẩu",
                    StatusCode = StatusCodes.Status200OK
                });
            }catch(Exception ex)
            {
                Log.Error($"Lỗi khác: {ex.Message}");
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            
        }
    }
}
