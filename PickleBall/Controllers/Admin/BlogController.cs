using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service;

namespace PickleBall.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost("add-blog")]
        public async Task<IActionResult> Create([FromForm] BlogRequest blog)
        {
            try
            {
                await _blogService.Create(blog);

                return Ok(new
                {
                    Message = "Thêm mới thành công",
                    StatusCode = StatusCodes.Status201Created
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

        [HttpPut("update-blog/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] BlogRequest blog)
        {
            try
            {
                await _blogService.Update(id, blog);

                return Ok(new
                {
                    Message = "Cập nhật thành công",
                    StatusCode = StatusCodes.Status200OK
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

        [HttpPatch("delete-blog/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _blogService.Delete(id);

                return Ok(new
                {
                    Message = "Xóa thành công",
                    StatusCode = StatusCodes.Status200OK
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
