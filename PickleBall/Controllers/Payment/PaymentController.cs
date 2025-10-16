using DotNetEnv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using PickleBall.Dto.Request;
using PickleBall.Service.Checkout;
using Serilog;

namespace PickleBall.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IPayOSService _payOS;
        public PaymentController(ICheckoutService checkoutService, IPayOSService payOS)
        {
            _checkoutService = checkoutService;
            _payOS = payOS;          
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] BookingRequest booking)
        {
            try
            {
                var result = await _checkoutService.Checkout(booking);

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
                    Message = "Tạo đơn thành công",
                    StatusCode = result.StatusCode,
                    Data = result.Data
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác : ${ex.Message}");
                return BadRequest(new
                {
                    message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest,
                });
            }
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmWebhook([FromBody] Guid partnerId)
        {
            var result = await _payOS.ConfirmPayOSWebHook(partnerId);
            return Ok(result);
        }
    }
}
