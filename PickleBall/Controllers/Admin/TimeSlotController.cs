using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service;

namespace PickleBall.Controllers.Admin
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

        [HttpPost("add-timeslot")]
        public async Task<IActionResult> Add(TimeSlotRequest timeSlot)
        {
            try
            {
                await _timeSlotService.Add(timeSlot);

                return Ok(new
                {
                    Message = "Thêm mới thành công",
                    StatusCode = StatusCodes.Status200OK
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
            catch (Exception ex) {
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpDelete("delete-timeslot")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _timeSlotService.Delete(id);

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
