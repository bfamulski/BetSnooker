using System;
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
            var result = await _betsService.GetUserBets(userId);
            if (result == null)
            {
                return NoContent(); // round already started
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBets()
        {
            var result = await _betsService.GetAllBets();
            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] RoundBets bets)
        {
            var userId = GetUserIdFromRequest(Request);
            var result = await _betsService.SubmitBets(userId, bets);
            if (!result)
            {
                return BadRequest(new { message = "Could not submit bets for current round, because it has already started." });
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