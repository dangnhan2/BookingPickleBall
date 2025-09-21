using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service.Courts;
using Serilog;

namespace PickleBall.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class CourtController : ControllerBase
    {
        private readonly ICourtService _courtService;
        public CourtController(ICourtService courtService)
        {
            _courtService = courtService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CourtRequest court)
        {
            try
            {
                var result = await _courtService.Add(court);

                if (!result.Success) {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = result.StatusCode,
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
        public async Task<IActionResult> Update(Guid id,[FromForm] CourtRequest court)
        {
            try
            {
                var result =  await _courtService.Update(id, court);

                if (!result.Success) {
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _courtService.Delete(id);

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
