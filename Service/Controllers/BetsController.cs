using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BetSnooker.Controllers
{
    /// <summary>
    /// Bets controller.
    /// </summary>
    /// <remarks>
    /// Only authorized users are allowed.
    /// </remarks>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BetsController : ControllerBase
    {
        private readonly IBetsService _betsService;
        private readonly ILogger _logger;

        public BetsController(IBetsService betsService, ILogger<BetsController> logger)
        {
            _betsService = betsService;
            _logger = logger;
        }

        /// <summary>
        /// Get all user bets.
        /// </summary>
        /// <returns>User bets</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<RoundBets>))]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetUserBets()
        {
            var userId = GetUserIdFromRequest(Request);
            _logger.LogDebug($"Getting all bets for user: {userId}");

            var bets = await _betsService.GetUserBets(userId);
            if (bets == null || !bets.Any())  // TODO: check for matches in each roundBet
            {
                return NoContent();
            }

            return Ok(bets);
        }

        /// <summary>
        /// Get all bets from all users. Bets for finished or ongoing rounds are returned.
        /// </summary>
        /// <returns>All bets</returns>
        /// <remarks>
        /// This method is not authorized - any user can call it.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventBets>))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetEventBets()
        {
            _logger.LogDebug("Getting all bets for all users");
            var bets = await _betsService.GetEventBets();
            if (bets == null || !bets.Any())
            {
                return NoContent();
            }

            return Ok(bets);
        }

        /// <summary>
        /// Submit user bets.
        /// </summary>
        /// <param name="bets">User bets</param>
        /// <returns>Submit result</returns>
        [Obsolete("Use the newer version of the endpoint")]
        [HttpPost("v1")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SubmitV1([FromBody] RoundBets bets)
        {
            var userId = GetUserIdFromRequest(Request);
            _logger.LogInformation($"Submitting round bets (round ID: {bets.RoundId}) for user: {userId}");
            
            var result = await _betsService.SubmitBets(userId, bets);
            switch (result)
            {
                case SubmitResult.ValidationError:
                    _logger.LogError($"Invalid bets (round ID: {bets.RoundId}, user: {userId})");
                    return BadRequest(new { message = "Invalid bets" });
                case SubmitResult.InvalidRound:
                    _logger.LogError($"Invalid round or round already started (round ID: {bets.RoundId}, user: {userId})");
                    return BadRequest(new { message = "Invalid round or round already started" });
                case SubmitResult.InternalServerError:
                    _logger.LogError($"Unexpected error while submitting bets (round ID: {bets.RoundId}, user: {userId})");
                    return BadRequest(new { message = "Unexpected error while submitting bets" });
                default:
                    _logger.LogInformation($"Bets submitted successfully (round ID: {bets.RoundId}, user: {userId})");
                    return Ok();
            }
        }

        /// <summary>
        /// Submit user bets.
        /// </summary>
        /// <param name="bets">User bets</param>
        /// <returns>Submit result</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Submit([FromBody] IEnumerable<RoundBets> bets)
        {
            var userId = GetUserIdFromRequest(Request);

            foreach (var roundBets in bets)
            {
                if (!roundBets.MatchBets.Any())
                {
                    _logger.LogInformation($"No match bets to submit (round ID: {roundBets.RoundId}) for user: {userId}");
                    continue;
                }

                _logger.LogInformation($"Submitting round bets (round ID: {roundBets.RoundId}) for user: {userId}");

                var result = await _betsService.SubmitBets(userId, roundBets);
                switch (result)
                {
                    case SubmitResult.ValidationError:
                        _logger.LogError($"Invalid bets (round ID: {roundBets.RoundId}, user: {userId})");
                        return BadRequest(new { message = "Invalid bets" });
                    case SubmitResult.InvalidRound:
                        _logger.LogError($"Invalid round or round already started (round ID: {roundBets.RoundId}, user: {userId})");
                        return BadRequest(new { message = "Invalid round or round already started" });
                    case SubmitResult.InternalServerError:
                        _logger.LogError($"Unexpected error while submitting bets (round ID: {roundBets.RoundId}, user: {userId})");
                        return BadRequest(new { message = "Unexpected error while submitting bets" });
                    default:
                        _logger.LogInformation($"Bets submitted successfully (round ID: {roundBets.RoundId}, user: {userId})");
                        break;
                }
            }

            return Ok();
        }

        private string GetUserIdFromRequest(HttpRequest request)
        {
            var authHeader = AuthenticationHeaderValue.Parse(request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            var username = credentials[0];
            return username;
        }
    }
}