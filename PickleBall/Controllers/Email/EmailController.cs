using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Models;

namespace PickleBall.Controllers.Email
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly UserManager<Partner> _userManager;

        public EmailController(UserManager<Partner> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> SendEmailConfirm(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
              return Redirect("https://pickleboom.vercel.app/login?confirm=fail");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
              await _userManager.DeleteAsync(user);
              return Redirect("https://pickleboom.vercel.app/login?confirm=fail");
            }

            user.EmailConfirmed = true;

             return Redirect("https://pickleboom.vercel.app/login?confirm=success");
        }
        
    }
}
