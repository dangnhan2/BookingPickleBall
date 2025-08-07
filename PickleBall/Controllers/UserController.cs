using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service;
using Serilog;
using System.Security.Claims;

namespace PickleBall.Controllers
{
    //[Authorize]   
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var result = await _userService.GetById(id);

                if (result.Success != true)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = StatusCodes.Status404NotFound
                    });
                }

                return Ok(new
                {
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result.Data
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi lấy thông tin người dùng bằng id : ${ex.InnerException.Message ?? ex.Message}");

                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPut("upload-avatar/{userId}")]
        public async Task<IActionResult> UploadAvatar(string userId, IFormFile file)
        {
            try
            {  
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == id)
                {
                    await _userService.UploadAvatarByUser(userId, file);

                    return Ok(new
                    {
                        Message = "Cập nhật hình ảnh thành công",
                        StatusCode = StatusCodes.Status200OK,
                    });
                }

                return Unauthorized(new
                {
                    Message = "Id của người dùng không hợp lệ",
                    StatusCode = StatusCodes.Status401Unauthorized
                });
            }
            catch (KeyNotFoundException ex) {
                return NotFound(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi cập nhật user avatar : ${ex.InnerException.Message ?? ex.Message}");

                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserRequest user)
        {
            try
            {
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (id == userId)
                {
                    var result = await _userService.UpdateByUser(userId, user);

                    if (result.Success != true)
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

                return Unauthorized(new
                {
                    Message = "Id của người dùng không hợp lệ",
                    StatusCode = StatusCodes.Status401Unauthorized
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi cập nhật user : ${ex.InnerException.Message ?? ex.Message}");

                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

       
    }
}
