using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using PickleBall.Models.Enum;
using PickleBall.Service.BackgoundJob;
using PickleBall.Service.Checkout;
using PickleBall.Service.SignalR;
using PickleBall.UnitOfWork;
using System.Security.Cryptography;
using System.Text;

namespace PickleBall.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly IPayOSService _payOSService;
        public WebHookController(IPayOSService payOSService, IUnitOfWorks unitOfWorks, IHubContext<BookingHub> context)
        {
            _payOSService = payOSService;         
        }

        [HttpPost]
        public async Task<IActionResult> CallBack()
        {
            //var result = await _payOSService.CallBack(Request);

            //if (!result.Success)
            //{
            //    return BadRequest(new
            //    {
            //        Message = result.Error,
            //        StatusCode = result.StatusCode
            //    });
            //}

            //return Ok(new
            //{
            //    Message = result.Data,
            //    StatusCode = result.StatusCode
            //});
            return Ok(new { message = "hi" });
        }
    }
}
