using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Services.Interfaces;
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
            var result = await Task.Run(() => _snookerFeedService.GetCurrentEvent());
            return Ok(result);
        }

        [HttpGet("matches/all")]
        public async Task<IActionResult> GetEventMatches()
        {
            var matches = await Task.Run(() => _snookerFeedService.GetEventMatches());
            if (matches == null || !matches.Any())
            {
                return NoContent();
            }
            
            return Ok(matches);
        }

        [HttpGet("rounds/current")]
        public async Task<IActionResult> GetCurrentRoundInfo()
        {
            var result = await Task.Run(() => _snookerFeedService.GetCurrentRound());
            return Ok(result);
        }

        [HttpGet("rounds/all")]
        public async Task<IActionResult> GetEventRounds()
        {
            var rounds = await Task.Run(() => _snookerFeedService.GetEventRounds());
            if (rounds == null || !rounds.Any())
            {
                return NoContent();
            }

            return Ok(rounds);
        }
    }
}