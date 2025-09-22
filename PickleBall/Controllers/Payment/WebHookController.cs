using DotNetEnv;
using Microsoft.AspNetCore.Mvc;

namespace PickleBall.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly string _secretKey;

        public WebHookController()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            Env.Load($".env.{env.ToLower()}");
            _secretKey = Env.GetString("PAYOS_CHECKSUM_KEY");
        }

        [HttpPost]
        public async Task<IActionResult> Callback()
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();

            Console.WriteLine("Webhook raw body: " + rawBody);

            // TODO: parse rawBody → JObject / Dictionary
            // TODO: lấy signature từ body và verify bằng VerifySignature()

            return Ok(new { message = "Webhook received" });
        }
    }
}
