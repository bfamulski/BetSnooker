using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BetSnooker.Models.API;
using BetSnooker.Services;
using Microsoft.AspNetCore.Mvc;

namespace BetSnooker.Controllers
{
    // TODO: this should be authorized
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("currentEvent")]
        public async Task<IActionResult> GetCurrentEvent()
        {
            var result = await _adminService.GetCurrentEvent();
            return Ok(result);
        }

        [HttpPost("currentEvent")]
        public async Task<IActionResult> SetCurrentEvent([Required, FromBody] Event @event)
        {
            await _adminService.SetCurrentEvent(@event);
            return Ok();
        }

        [HttpPost("startRound")]
        public async Task<IActionResult> SetStartRound([Required, FromBody] StartRoundRequest request)
        {
            await _adminService.SetStartRound(request.RoundId);
            return Ok();
        }
    }

    public class StartRoundRequest
    {
        public int RoundId { get; set; }
    }
}