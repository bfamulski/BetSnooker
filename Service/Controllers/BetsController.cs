using System;
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

        [HttpGet]
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

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBets()
        {
            var bets = await _betsService.GetAllBets();
            if (bets == null || !bets.Any())
            {
                return NoContent();
            }

            return Ok(bets);
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] RoundBets bets)
        {
            var userId = GetUserIdFromRequest(Request);
            var result = await _betsService.SubmitBets(userId, bets);
            if (!result)
            {
                return BadRequest("Could not submit bets for current round, because it has already started.");
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