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

        [HttpGet("events/{season:int}")]
        public async Task<IActionResult> GetEvents(int season)
        {
            var result = await _snookerFeedService.GetEvents(season);
            return Ok(result);
        }

        [HttpGet("matches/{roundId:int}")]
        public async Task<IActionResult> GetRoundMatches(int roundId)
        {
            var result = await _snookerFeedService.GetRoundMatches(roundId);
            return Ok(result);
        }

        [HttpGet("matches/all")]
        public async Task<IActionResult> GetEventMatches()
        {
            var result = await _snookerFeedService.GetEventMatches();
            return Ok(result);
        }

        [HttpGet("round/{roundId:int}")]
        public async Task<IActionResult> GetRoundInfo(int roundId)
        {
            var result = await _snookerFeedService.GetRoundInfo(roundId);
            return Ok(result);
        }
    }
}