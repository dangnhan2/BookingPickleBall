using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service;

namespace PickleBall.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly ICourtService _courtService;
        public CourtController(ICourtService courtService)
        {
            _courtService = courtService;
        }

        [HttpPost("add-court")]
        public async Task<IActionResult> Create([FromForm] CourtRequest court)
        {
            try
            {
                await _courtService.Add(court);

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

        [HttpPut("update-court/{id}")]
        public async Task<IActionResult> Update(Guid id,[FromForm] CourtRequest court)
        {
            try
            {
                await _courtService.Update(id, court);

                return Ok(new
                {
                    Message = "Cập nhật thành công",
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
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new
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

        [HttpPatch("delete-court/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _courtService.Delete(id);

                return Ok(new
                {
                    Message = "Xóa thành công",
                    StatusCode = StatusCodes.Status201Created
                });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new
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
