using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Service;
using Serilog;

namespace PickleBall.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;

        public TimeSlotController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _timeSlotService.GetAll();

                return Ok(new
                {
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác : {ex.InnerException.Message ?? ex.Message}");
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpGet("{courtId}")]
        public async Task<IActionResult> GetTimeSlots(Guid courtId,[FromQuery] DateOnly date)
        {
            var result = await _timeSlotService.GetAllBooked(courtId, date);

            return Ok(new
            {
                Message = "Lấy dữ liệu thành công",
                StatusCode = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
