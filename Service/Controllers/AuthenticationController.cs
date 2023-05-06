using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger _logger;

        public AuthenticationController(IAuthenticationService authenticationService, ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
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
            _logger.LogInformation($"Registering new user: {user.Username}");

            var result = await _authenticationService.Register(user);
            if (result == null)
            {
                var errorMessage = "Could not register user";
                _logger.LogError(errorMessage);
                return BadRequest(new { message = errorMessage });
            }

            _logger.LogInformation($"User '{result.Username}' registered successfully");
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
            _logger.LogInformation($"Logging in user: {credentials.Username}");

            var user = await _authenticationService.Login(credentials);
            if (user == null)
            {
                var errorMessage = "Username or password is incorrect";
                _logger.LogError(errorMessage);
                return BadRequest(new { message = errorMessage });
            }

            _logger.LogInformation($"User '{user.Username}' logged in successfully");
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
            _logger.LogDebug("Getting users");

            var users = await _authenticationService.GetUsers();
            if (users == null || !users.Any())
            {
                _logger.LogWarning("No users available");
                return NoContent();
            }

            _logger.LogDebug($"Found users: {string.Join(',', users.Select(u => u.Username))}");
            return Ok(users);
        }
    }
}