using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BetSnooker.Configuration;
using BetSnooker.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BetSnooker.Services
{
    public class NotificationsHubService : INotificationsHubService
    {
        private static readonly TimeSpan GetMatchesTimerPeriod = TimeSpan.FromMinutes(30);

        private Timer _matchesTimer;

        private readonly IServiceProvider _serviceProvider;
        private readonly ISnookerHubService _snookerHubService;
        private readonly ISettingsProvider _settingsProvider;

        private readonly ILogger _logger;

        public NotificationsHubService(
            IServiceProvider serviceProvider,
            ISnookerHubService snookerHubService,
            ISettingsProvider settingsProvider,
            ILogger<NotificationsHubService> logger)
        {
            _serviceProvider = serviceProvider;
            _snookerHubService = snookerHubService;
            _settingsProvider = settingsProvider;
            _logger = logger;
        }

        public void StartHub()
        {
            _logger.LogInformation($"Starting {nameof(NotificationsHubService)}");
            _matchesTimer = new Timer(GetMatchesTimerEvent, null, TimeSpan.FromSeconds(5), GetMatchesTimerPeriod);
            _logger.LogInformation($"{nameof(NotificationsHubService)} started");
        }

        public void DisposeHub()
        {
            _logger.LogInformation($"Disposing {nameof(NotificationsHubService)}");
            _matchesTimer?.Dispose();
            _logger.LogInformation($"{nameof(NotificationsHubService)} disposed");
        }

        private async void GetMatchesTimerEvent(object obj)
        {
            _logger.LogInformation($"{nameof(NotificationsHubService)}: get event matches");
            var eventMatches = _snookerHubService.GetEventMatches().Where(m =>
                m.EventId == _settingsProvider.EventId && m.Round >= _settingsProvider.StartRound).ToList();
            if (eventMatches.Any())
            {
                if (eventMatches.All(m => m.StartDate.HasValue))
                {
                    DisposeHub();
                    return;
                }

                var incomingMatchesCount = eventMatches.Count(m =>
                    m.ScheduledDate.HasValue && m.ScheduledDate.Value.TimeOfDay != TimeSpan.Zero &&
                    !m.StartDate.HasValue &&
                    m.ScheduledDate.Value - DateTime.UtcNow < TimeSpan.FromMinutes(60));

                if (incomingMatchesCount > 0)
                {
                    await SendNotification("It's time to place your bets!");
                }
            }
        }

        private async Task SendNotification(string message)
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationsService>();
            if (notificationsService != null)
            {
                await notificationsService.SendNotification(message);
            }
        }
    }
}