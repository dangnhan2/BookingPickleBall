using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.Request;
using PickleBall.Service.SoftService;
using Serilog;

namespace PickleBall.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PaymentLinkController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public PaymentLinkController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] BookingRequest booking)
        {
            try
            {
                var result = await _checkoutService.Checkout(booking);

                return Ok(new
                {
                    Message = "Tạo đơn thành công",
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
                    StatusCode = StatusCodes.Status400BadRequest,
                });
            }
        }
    }
}
