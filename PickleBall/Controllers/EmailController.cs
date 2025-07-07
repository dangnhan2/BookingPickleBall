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
                return BadRequest(new {
                   Message = "Người dùng không tồn tại",
                   StatusCode = StatusCodes.Status400BadRequest
                });

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return BadRequest(new
                {
                    Message = "Email confirmation failed. Token may be expired or invalid.",
                    StatusCode = StatusCodes.Status400BadRequest
                });

            user.EmailConfirmed = true;

            return Ok(new
            {
                Message = "Xác nhận thành công",
                StatusCode = StatusCodes.Status200OK
            });
        }
    }
}
