using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;
using PickleBall.Service.Blogs;
using PickleBall.Service.Users;
using Serilog;

namespace PickleBall.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBlogService _blogService;

        public AdminController(IUserService userService, IBlogService blogService) { 
          _userService = userService;
          _blogService = blogService;
        }

        [HttpGet("user")]
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

        [HttpPatch("blog/{id}")]
        public async Task<IActionResult> DeleteBlog(Guid id)
        {
            try
            {
                var result = await _blogService.Delete(id);

                if (!result.Success)
                {
                    return NotFound(new
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
                Log.Error($"Lỗi khác : ${ex.Message}");

                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }
    }
}
