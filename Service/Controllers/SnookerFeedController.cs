using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Models.API;
using BetSnooker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BetSnooker.Controllers
{
    /// <summary>
    /// SnookerFeed controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class SnookerFeedController : ControllerBase
    {
        private readonly ISnookerFeedService _snookerFeedService;

        public SnookerFeedController(ISnookerFeedService snookerFeedService)
        {
            _snookerFeedService = snookerFeedService;
        }

        /// <summary>
        /// Get current event data.
        /// </summary>
        /// <returns>Event object</returns>
        [HttpGet("events/current")]
        [ProducesResponseType(200, Type = typeof(Event))]
        public async Task<IActionResult> GetCurrentEvent()
        {
            var result = await Task.Run(() => _snookerFeedService.GetCurrentEvent());
            return Ok(result);
        }

        /// <summary>
        /// Get event matches starting from provided start round.
        /// </summary>
        /// <returns>Collection of matches</returns>
        [HttpGet("matches/all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MatchDetails>))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetEventMatches()
        {
            var matches = await Task.Run(() => _snookerFeedService.GetEventMatches());
            if (matches == null || !matches.Any())
            {
                return NoContent();
            }
            
            return Ok(matches);
        }

        /// <summary>
        /// Get current event round.
        /// </summary>
        /// <returns>RoundInfo details</returns>
        [HttpGet("rounds/current")]
        [ProducesResponseType(200, Type = typeof(RoundInfoDetails))]
        public async Task<IActionResult> GetCurrentRoundInfo()
        {
            var result = await Task.Run(() => _snookerFeedService.GetCurrentRound());
            return Ok(result);
        }

        /// <summary>
        /// Get event rounds starting from provided start round.
        /// </summary>
        /// <returns>Collection of rounds</returns>
        [HttpGet("rounds/all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<RoundInfo>))]
        [ProducesResponseType(204)]
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