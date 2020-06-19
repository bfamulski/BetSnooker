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

        public BetsController(IBetsService betsService)
        {
            _betsService = betsService;
        }

        /// <summary>
        /// Get all user bets.
        /// </summary>
        /// <returns>User bets</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(RoundBets))]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetUserBets()
        { 
            var userId = GetUserIdFromRequest(Request);
            var bets = await _betsService.GetUserBets(userId);
            if (bets == null || !bets.MatchBets.Any())
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
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Submit([FromBody] RoundBets bets)
        {
            var userId = GetUserIdFromRequest(Request);
            var result = await _betsService.SubmitBets(userId, bets);
            switch (result)
            {
                case SubmitResult.ValidationError:
                    return BadRequest(new { message = "Invalid bets" });
                case SubmitResult.InvalidRound:
                    return BadRequest(new { message = "Invalid round or round already started" });
                case SubmitResult.InternalServerError:
                    return BadRequest(new { message = "Unexpected error while submitting bets" });
                default:
                    return Ok();
            }
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