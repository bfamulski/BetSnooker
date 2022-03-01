using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BetSnooker.Controllers
{
    /// <summary>
    /// Notifications controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService _notificationsService;
        private readonly ILogger _logger;

        public NotificationsController(INotificationsService notificationsService, ILogger<NotificationsController> logger)
        {
            _notificationsService = notificationsService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe([FromBody, Required] NotificationSubscription subscription)
        {
            _logger.LogDebug("Adding user subscription");
            await _notificationsService.AddSubscription(subscription);
            return Ok();
        }
    }
}