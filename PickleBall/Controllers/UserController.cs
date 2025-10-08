using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service.Courts;
using PickleBall.Service.Users;
using Serilog;
using System.Security.Claims;

namespace PickleBall.Controllers
{
    [Authorize]   
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICourtService _courtService;
        public UserController(IUserService userService, ICourtService courtService)
        {
            _userService = userService;
            _courtService = courtService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _userService.GetById(id);

                if (result.Success != true)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = result.StatusCode
                    });
                }

                return Ok(new
                {
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = result.StatusCode,
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

        [HttpPut("upload-avatar/{id}")]
        public async Task<IActionResult> UploadAvatar(Guid id, IFormFile file)
        {
            try
            {

                await _userService.UploadAvatarByUser(id, file);

                return Ok(new
                {
                    Message = "Cập nhật hình ảnh thành công",
                    StatusCode = StatusCodes.Status200OK,
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
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserRequest user)
        {
            try
            {
                    var result = await _userService.UpdateByUser(id, user);

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
