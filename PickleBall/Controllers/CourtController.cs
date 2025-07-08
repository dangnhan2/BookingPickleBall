using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.QueryParams;
using PickleBall.Service;

namespace PickleBall.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly ICourtService _courtService;

        public CourtController(ICourtService courtService) { 
           _courtService = courtService;    
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll([FromQuery] CourtParams court)
        {
            try
            {
                var result = await _courtService.GetAll(court);

                return Ok(new
                {
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
                });
            }catch (Exception ex)
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
                var result = await _courtService.GetById(id);

                return Ok(new
                {
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
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
