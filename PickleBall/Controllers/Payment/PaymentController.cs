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
        public async Task<IActionResult> ConfirmWebhook()
        {
            var result = await _payOS.ConfirmPayOSWebHook();
            return Ok(result);
        }
    }
}

//{
//    "userID": "7bb7fd56-9a8b-44e9-8529-a61115c3730a",
//  "courtID": "fa11eb86-27a6-4833-89f2-2e6073b09000",
//  "bookingDate": "2025-09-23",
//  "customerName": "Nguyễn Đăng Nhân",
//  "amount": 1000,
//  "timeSlots": [
//    "dd7e8d16-fe42-4816-a90c-7d9856749f3e",
//    "97d52ff5-76e3-4a81-a59b-9fff0835fed8"
//  ]
//}
