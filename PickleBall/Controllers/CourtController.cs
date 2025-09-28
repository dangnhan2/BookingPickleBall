using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto.QueryParams;
using PickleBall.Service.Courts;
using Serilog;

namespace PickleBall.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly ICourtService _courtService;

        public CourtController(ICourtService courtService) { 
           _courtService = courtService;    
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInSpecificDate(Guid id, DateOnly date)
        {
            try
            {
                var result = await _courtService.GetAllInSpecificDate(id, date);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }
    }
}
//{
//    "email": "partner@gmail.com",
//  "password": "123456"
//}