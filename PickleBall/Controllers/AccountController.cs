using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service.Auth;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace PickleBall.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var result = await _accountService.Login(request, HttpContext);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                       Message = result.Error,
                       StatusCode = result.StatusCode
                    });
                }

                return Ok(new
                {
                    Message = "Đăng nhập thành công",
                    StatusCode = result.StatusCode,
                    AccessToken = result.Data
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
                        StatusCode = result.StatusCode
                    });
                }

                return Ok(new
                {
                    Message = result.Data,
                    StatusCode = result.StatusCode
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác : {ex.InnerException.Message ?? ex.Message}");
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("admin-register")]
        public async Task<IActionResult> RegisterForAdmin(RegisterRequest request)
        {
            try
            {
                var result = await _accountService.RegisterForAdmin(request);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = result.StatusCode
                    });
                }

                return Ok(new
                {
                     Message = result.Data,
                     StatusCode = result.StatusCode
                });
            }catch(Exception ex)
            {
                Log.Error($"Lỗi không đăng kí tài khoàn cho role admin: {ex.InnerException.Message ?? ex.Message}");

                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
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
                var result = await _accountService.ChangePassword(userId, passwordRequest, HttpContext);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = result.StatusCode
                    });
                }
                return Ok(new
                {
                    Message = result.Data,
                    StatusCode = result.StatusCode
                });
            }catch (Exception ex)
            {   
                Log.Error($"Lỗi khác: {ex.InnerException.Message ?? ex.Message}");
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {           
            try
            {   
                var result = await _accountService.Logout(HttpContext);

                if (!result.Success)
                {
                    Log.Error(result.Error);
                    return Unauthorized(new
                    {
                        Message = result.Error,
                        StatusCode = result.StatusCode
                    });
                }

                return Ok(new
                {
                    Message = result.Data,
                    StatusCode = result.StatusCode
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác: {ex.InnerException.Message ?? ex.Message}");
                return BadRequest(new
                {
                    Message = ex?.InnerException?.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [Authorize]               
        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {

            try
            {
                var result = await _accountService.RefreshToken(HttpContext);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = result.StatusCode
                    });
                }

                return Ok(new
                {
                    Message = "Refresh token succeed",
                    StatusCode = result.StatusCode,
                    AccessToken = result.Data
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
