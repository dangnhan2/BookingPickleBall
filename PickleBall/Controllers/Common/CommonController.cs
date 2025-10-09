using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.QueryParams;
using PickleBall.Service.Blogs;
using PickleBall.Service.Courts;
using Serilog;

namespace PickleBall.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICourtService _courtService;
        private readonly IBlogService _blogService;
        public CommonController(ICourtService courtService, IBlogService blogService) { 
           _courtService = courtService;
           _blogService = blogService;
        }

        [HttpGet("courts")]
        public async Task<IActionResult> GetAllCourt()
        {
            var result = await _courtService.GetAllPartnerInfo();

            return Ok(new
            {
                Message = "Lấy dữ liệu thành công",
                StatusCode = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("courts/{id}")]
        public async Task<IActionResult> GetAllInSpecificDate(Guid id, DateOnly date)
        {
            try
            {
                var result = await _courtService.GetAllInSpecificDate(id, date);

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
                    Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpGet("blog")]
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
                Log.Error($"Lỗi khác : ${ex.InnerException.Message ?? ex.Message}");

                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpGet("blog/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _blogService.GetById(id);

                if (result.Success != true)
                {
                    return NotFound(new
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
                Log.Error($"Lỗi khác : ${ex.InnerException.Message ?? ex.Message}");

                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }
    }
}
//{
//    "email": "partner@gmail.com",
//  "password": "123456"
//}