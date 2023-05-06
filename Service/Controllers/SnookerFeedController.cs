using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Models.API;
using BetSnooker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger _logger;

        public SnookerFeedController(ISnookerFeedService snookerFeedService, ILogger<SnookerFeedController> logger)
        {
            _snookerFeedService = snookerFeedService;
            _logger = logger;
        }

        /// <summary>
        /// Get current event data.
        /// </summary>
        /// <returns>Event object</returns>
        [HttpGet("events/current")]
        [ProducesResponseType(200, Type = typeof(Event))]
        public async Task<IActionResult> GetCurrentEvent()
        {
            _logger.LogDebug("Getting current event");

            try
            {
                var result = await _snookerFeedService.GetCurrentEvent();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
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
            _logger.LogDebug("Getting event matches");

            try
            {
                var matches = await _snookerFeedService.GetEventMatches();
                if (matches == null || !matches.Any())
                {
                    return NoContent();
                }

                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get ongoing matches.
        /// </summary>
        /// <returns>Collection of matches</returns>
        [HttpGet("matches/ongoing")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MatchDetails>))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetOngoingMatches()
        {
            _logger.LogDebug("Getting ongoing matches");

            try
            {
                var matches = await _snookerFeedService.GetOngoingMatches();
                if (matches == null || !matches.Any())
                {
                    return NoContent();
                }

                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get current event round.
        /// </summary>
        /// <returns>RoundInfo details</returns>
        [HttpGet("rounds/current")]
        [ProducesResponseType(200, Type = typeof(RoundInfoDetails))]
        public async Task<IActionResult> GetCurrentRoundInfo()
        {
            _logger.LogDebug("Getting current round");

            try
            {
                var result = await _snookerFeedService.GetCurrentRound(null);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get event rounds starting from provided start round.
        /// </summary>
        /// <returns>Collection of rounds</returns>
        [HttpGet("rounds/all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<RoundInfoDetails>))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetEventRounds()
        {
            _logger.LogDebug("Getting event rounds");

            try
            {
                var rounds = await _snookerFeedService.GetEventRounds();
                if (rounds == null || !rounds.Any())
                {
                    return NoContent();
                }

                return Ok(rounds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}