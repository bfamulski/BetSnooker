using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BetSnooker.Controllers
{
    /// <summary>
    /// Authentication controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Register new user.
        /// </summary>
        /// <param name="user">New user data</param>
        /// <returns>Registered user data</returns>
        [HttpPost("register")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([Required, FromBody] User user)
        {
            var result = await _authenticationService.Register(user);
            if (result == null)
            {
                return BadRequest(new { message = "Could not register user" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Log in user.
        /// </summary>
        /// <param name="credentials">User credentials</param>
        /// <returns>Logged in user data</returns>
        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([Required, FromBody] Credentials credentials)
        {
            var user = await _authenticationService.Login(credentials);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Get registered users.
        /// </summary>
        /// <returns>Collection of registered users</returns>
        [HttpGet("users")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(204)]
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