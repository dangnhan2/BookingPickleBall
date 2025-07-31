using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service;
using System.Security.Claims;

namespace PickleBall.Controllers
{
    [Authorize]   
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

                return Ok(new
                {
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status404NotFound
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
                return BadRequest(new
                {
                    Message = ex.Message,
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
                    await _userService.UpdateByUser(userId, user);

                    return Ok(new
                    {
                        Message = "Cập nhật thành công",
                        StatusCode = StatusCodes.Status200OK,
                    });
                }                    

                return Unauthorized(new
                {
                    Message = "Id của người dùng không hợp lệ",
                    StatusCode = StatusCodes.Status401Unauthorized
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status404NotFound
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
