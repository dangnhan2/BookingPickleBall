using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service.Auth;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace PickleBall.Controllers.Auth
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
                    result.Data
                });

            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác: {ex.Message ?? ex.InnerException.Message}");
                return BadRequest(new
                {
                    ex.Message,
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
                        result.StatusCode
                    });
                }
                return Ok(new
                {
                    Message = result.Data,
                    result.StatusCode
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
                        result.StatusCode
                    });
                }

                return Ok(new
                {
                    Message = result.Data,
                    result.StatusCode
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
                        result.StatusCode
                    });
                }

                return Ok(new
                {
                    Message = "Refresh token succeed",
                    result.StatusCode,
                    result.Data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        //[Authorize]
        [HttpPost("register-partner")]
        public async Task<IActionResult> RegisterParterByAdmin(RegisterPartnerRequest request)
        {
            try
            {
                var result = await _accountService.CreatePartnerByAdmin(request);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        result.StatusCode
                    });
                }

                return Ok(new
                {
                    Message = result.Data,
                    result.StatusCode,
                });

            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác: {ex.Message ?? ex.InnerException.Message}");
                return BadRequest(new
                {
                    ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("admin-register")]
        public async Task<IActionResult> AdminRegister([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _accountService.CreateAdmin(request);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        result.StatusCode
                    });
                }

                return Ok(new
                {
                    Message = result.Data,
                    result.StatusCode,
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác: {ex.Message ?? ex.InnerException.Message}");
                return BadRequest(new
                {
                    ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }
       
    }
}
