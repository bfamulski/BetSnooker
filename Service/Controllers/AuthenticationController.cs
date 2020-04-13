using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BetSnooker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([Required, FromBody] User user)
        {
            var result = await _authenticationService.Register(user);
            if (result == null)
            {
                return BadRequest(new { message = "Could not register user" });
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([Required, FromBody] Credentials credentials)
        {
            var user = await _authenticationService.Login(credentials);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return Ok(user);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _authenticationService.GetUsers();
            if (users == null || !users.Any())
            {
                return NoContent();
            }

            return Ok(users);
        }
    }
}