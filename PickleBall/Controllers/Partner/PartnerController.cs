using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;
using PickleBall.Service.Blogs;
using PickleBall.Service.Bookings;
using PickleBall.Service.Courts;
using PickleBall.Service.DashboardOverview;
using PickleBall.Service.TimeSlots;
using Serilog;

namespace PickleBall.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Partner")]
    public class PartnerController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly IBookingService _bookingService;
        private readonly ICourtService _courtService;
        private readonly ITimeSlotService _timeSlotService;
        private readonly IDashBoardService _dashBoardService;

        public PartnerController(IBlogService blogService, IBookingService bookingService, ICourtService courtService, ITimeSlotService timeSlotService, IDashBoardService dashBoardService)
        {
            _blogService = blogService;
            _bookingService = bookingService;
            _courtService = courtService;
            _timeSlotService = timeSlotService;
            _dashBoardService = dashBoardService;
        }

       
        [HttpPost("blog")]
        public async Task<IActionResult> CreateBlog([FromForm] BlogRequest blog)
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

        [HttpPut("blog/{id}")]
        public async Task<IActionResult> UpdateBlog(Guid id, [FromForm] BlogRequest blog)
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

        [HttpGet("booking-by-partner/{id}")]
        public async Task<IActionResult> GetBookings(Guid id, [FromQuery] BookingParams bookingParams)
        {
            try
            {
                var result = await _bookingService.GetByPartner(id, bookingParams);

                return Ok(new
                {
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác : {ex?.InnerException?.Message}");

                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpGet("court")]
        public async Task<IActionResult> GetAllByPartner(Guid id, [FromQuery] CourtParams court)
        {
            try
            {
                var result = await _courtService.GetAllByPartner(id, court);

                return Ok(new
                {
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
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
        [HttpGet("court/{id}")]
        public async Task<IActionResult> GetCourtById(Guid id)
        {
            try
            {
                var result = await _courtService.GetById(id);
                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        result.StatusCode
                    });
                }

                return Ok(new
                {
                    Message = "Lấy dữ liệu thành công",
                    result.StatusCode,
                    result.Data
                });
            }
            catch(Exception ex) {
                return BadRequest(new
                {
                    Message = ex.InnerException?.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("court")]
        public async Task<IActionResult> CreateCourt([FromForm] CourtRequest court)
        {
            try
            {
                var result = await _courtService.Add(court);

                if (!result.Success)
                {
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

        [HttpPut("court/{id}")]
        public async Task<IActionResult> UpdateCourt(Guid id, [FromForm] CourtRequest court)
        {
            try
            {
                var result = await _courtService.Update(id, court);

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

        [HttpDelete("court/{id}")]
        public async Task<IActionResult> DeleteCourt(Guid id)
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

        [HttpGet("timeslot")]
        public async Task<IActionResult> GetAll(Guid id)
        {
            try
            {
                var result = await _timeSlotService.GetByPartner(id);

                return Ok(new
                {
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác : {ex.InnerException?.Message ?? ex.Message}");
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("timeslot")]
        public async Task<IActionResult> AddTimeSlot(TimeSlotRequest timeSlot)
        {
            try
            {
                var result = await _timeSlotService.Add(timeSlot);

                if (result.Success != true)
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
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpDelete("timeslot/{id}")]
        public async Task<IActionResult> DeleteTimeSlot(Guid id)
        {
            try
            {
                var result = await _timeSlotService.Delete(id);

                if (result.Success != true)
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

        [HttpGet("dashboard")]
        public async Task<IActionResult> DashBoardOverviewByPartner(Guid id)
        {
            var result = await _dashBoardService.DashboardOverviewByPartner(id);

            return Ok(new
            {
                Message = "Lấy dữ liệu thành công",
                result.StatusCode,
                result.Data
            });
        }

    }
}
