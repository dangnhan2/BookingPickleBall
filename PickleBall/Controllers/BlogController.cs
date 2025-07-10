
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.QueryParams;
using PickleBall.Service;

namespace PickleBall.Controllers
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

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll([FromQuery] BlogParams blog)
        {
            try
            {
                var result = await _blogService.GetAll(blog);
                return Ok(new
                {
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
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

        [HttpGet("by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _blogService.GetById(id);
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
    }
}
