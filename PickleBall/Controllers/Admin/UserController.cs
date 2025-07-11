using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.QueryParams;
using PickleBall.Service;

namespace PickleBall.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) { 
          _userService = userService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll([FromQuery] UserParams user)
        {
            try
            {
                var result = await _userService.GetAll(user);

                return Ok(new
                {
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
                });
            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPatch("update-user/{userId}")]
        public async Task<IActionResult> UpdateByAdmin(string userId,[FromForm] UserRequestByAdmin user)
        {
            try
            {
                await _userService.UpdateByAdmin(userId, user);

                return Ok(new
                {
                    Message = "Cập nhật thành công",
                    StatusCode = StatusCodes.Status200OK,
                });
            }catch(KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
        }
    }
}
