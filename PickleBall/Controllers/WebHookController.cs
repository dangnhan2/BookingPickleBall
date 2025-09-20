using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Service.SoftService;
using Serilog;

namespace PickleBall.Controllers
{
    [Route("api/WebHook")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        //private readonly IPayOsService _payOsWebHookService;

        //public WebHookController(IPayOsService payOsWebHookService)
        //{
        //    _payOsWebHookService = payOsWebHookService;
        //}

        //[HttpPost]
        //public async Task<IActionResult> HandleWebHook(dynamic payload)
        //{
            
        //        await _payOsWebHookService.HanleWebHook(payload);

        //        return Ok();
            
        //}
    }
}
