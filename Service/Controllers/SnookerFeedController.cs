using System.Threading.Tasks;
using BetSnooker.Services;
using Microsoft.AspNetCore.Mvc;

namespace BetSnooker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SnookerFeedController : ControllerBase
    {
        private readonly ISnookerFeedService _snookerFeedService;

        public SnookerFeedController(ISnookerFeedService snookerFeedService)
        {
            _snookerFeedService = snookerFeedService;
        }

        [HttpGet("event/current")]
        public async Task<IActionResult> GetCurrentEvent()
        {
            var result = await _snookerFeedService.GetCurrentEvent();
            return Ok(result);
        }

        [HttpGet("matches/all")]
        public async Task<IActionResult> GetEventMatches()
        {
            var result = await _snookerFeedService.GetEventMatches();
            return Ok(result);
        }

        [HttpGet("round/current")]
        public async Task<IActionResult> GetCurrentRoundInfo()
        {
            var result = await _snookerFeedService.GetCurrentRound();
            return Ok(result);
        }
    }
}