using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service.Blogs;
using Serilog;

namespace PickleBall.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] BlogRequest blog)
        {
            try
            {
                var result = await _blogService.Create(blog);

                if(result.Success != true)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCodes = result.StatusCode
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
                Log.Error($"Lỗi khác : ${ex.Message}");

                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] BlogRequest blog)
        {
            try
            {
                var result =  await _blogService.Update(id, blog);

                if(!result.Success)
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
                Log.Error($"Lỗi khác : ${ex.Message}");

                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _blogService.Delete(id);

                if(!result.Success)
                {
                    return NotFound(new
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
