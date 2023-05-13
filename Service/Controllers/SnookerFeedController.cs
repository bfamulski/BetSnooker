using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Models.API;
using BetSnooker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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
            try
            {
                _logger.LogDebug("Getting current event");
                var result = await _snookerFeedService.GetCurrentEvent();
                _logger.LogDebug("Current event retrieved successfully");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
            try
            {
                _logger.LogDebug("Getting event matches");
                var matches = await _snookerFeedService.GetEventMatches();
                if (matches == null || !matches.Any())
                {
                    _logger.LogWarning("No event matches retrieved");
                    return NoContent();
                }

                _logger.LogDebug("Event matches retrieved successfully");
                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
            try
            {
                _logger.LogDebug("Getting ongoing matches");
                var matches = await _snookerFeedService.GetOngoingMatches();
                if (matches == null || !matches.Any())
                {
                    _logger.LogInformation("No ongoing matches retrieved");
                    return NoContent();
                }

                _logger.LogDebug("Ongoing matches retrieved successfully");
                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
            try
            {
                _logger.LogDebug("Getting current round");
                var result = await _snookerFeedService.GetCurrentRound(null);
                _logger.LogDebug("Current round retrieved successfully");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
            try
            {
                _logger.LogDebug("Getting event rounds");
                var rounds = await _snookerFeedService.GetEventRounds();
                if (rounds == null || !rounds.Any())
                {
                    _logger.LogWarning("No event rounds retrieved");
                    return NoContent();
                }

                _logger.LogDebug("Event rounds retrieved successfully");
                return Ok(rounds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}