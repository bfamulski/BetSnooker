using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Services;
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

        [HttpGet("{roundId:int}")]
        public async Task<IActionResult> Get(int roundId)
        { 
            var userId = GetUserIdFromRequest(Request);
            var result = await _betsService.GetBets(userId, roundId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("all/{roundId:int}")]
        public async Task<IActionResult> GetAllBets(int roundId)
        {
            var result = await _betsService.GetAllBets(roundId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] RoundBets bets)
        {
            var userId = GetUserIdFromRequest(Request);
            await _betsService.SubmitBets(userId, bets);
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