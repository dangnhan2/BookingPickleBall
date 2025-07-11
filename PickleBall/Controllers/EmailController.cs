using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Models;

namespace PickleBall.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public EmailController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> Send(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
              return Redirect("https://localhost:5173/login?confirm=fail");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
              return Redirect("https://localhost:5173/login?confirm=fail");

            user.EmailConfirmed = true;

             return Redirect("https://localhost:5173/login?confirm=success");
        }
    }
}
