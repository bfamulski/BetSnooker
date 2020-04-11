using System.Linq;
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

        [HttpGet("events/current")]
        public async Task<IActionResult> GetCurrentEvent()
        {
            var result = await _snookerFeedService.GetCurrentEvent();
            return Ok(result);
        }

        [HttpGet("matches/all")]
        public async Task<IActionResult> GetEventMatches()
        {
            var matches = await _snookerFeedService.GetEventMatches();
            if (matches == null || !matches.Any())
            {
                return BadRequest("No matches available at this moment");
            }
            
            return Ok(matches);
        }

        [HttpGet("rounds/current")]
        public async Task<IActionResult> GetCurrentRoundInfo()
        {
            var result = await _snookerFeedService.GetCurrentRound();
            if (result == null)
            {
                return NoContent();
                //return BadRequest("Current round is not yet available");
            }

            return Ok(result);
        }

        [HttpGet("rounds/all")]
        public async Task<IActionResult> GetEventRounds()
        {
            var rounds = await _snookerFeedService.GetEventRounds();
            if (rounds == null || !rounds.Any())
            {
                return NoContent();
            }

            return Ok(rounds);
        }
    }
}