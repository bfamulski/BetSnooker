using System;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Configuration;
using BetSnooker.Models;
using BetSnooker.Repositories.Interfaces;
using BetSnooker.Services.Interfaces;
using Microsoft.Extensions.Logging;
using WebPush;

namespace BetSnooker.Services
{
    public class NotificationsService : INotificationsService
    {
        private const string VapidSubject = "mailto:boguslaw.famulski@gmail.com";

        private readonly IUserSubscriptionsRepository _userSubscriptionsRepository;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ILogger _logger;

        public NotificationsService(IUserSubscriptionsRepository userSubscriptionsRepository, ISettingsProvider settingsProvider, ILogger<NotificationsService> logger)
        {
            _userSubscriptionsRepository = userSubscriptionsRepository;
            _settingsProvider = settingsProvider;
            _logger = logger;
        }

        public async Task AddSubscription(NotificationSubscription subscription)
        {
            var userSubscription = new UserSubscription
            {
                Endpoint = subscription.Endpoint,
                ExpirationTime = subscription.ExpirationTime,
                P256DH = subscription.Keys.P256DH,
                Auth = subscription.Keys.Auth
            };

            var result = await Task.Run(() => _userSubscriptionsRepository.Add(userSubscription));

            _logger.LogDebug(result ? "User subscription added" : "User subscription already exists");
        }

        public async Task SendNotification(string payloadMessage)
        {
            if (string.IsNullOrEmpty(_settingsProvider.VapidPublicKey) || string.IsNullOrEmpty(_settingsProvider.VapidPrivateKey))
            {
                throw new Exception("VAPID keys are not provided");
            }

            var subscriptions = _userSubscriptionsRepository.Get().ToList();
            if (!subscriptions.Any())
            {
                _logger.LogWarning("There are no user subscriptions");
                return;
            }

            var webPushClient = new WebPushClient();
            var vapidDetails = new VapidDetails(VapidSubject, _settingsProvider.VapidPublicKey, _settingsProvider.VapidPrivateKey);

            var payload = $"{{ \"notification\": {{ \"title\": \"BetSnooker\", \"body\": \"{payloadMessage}\" }} }}";

            foreach (var subscription in subscriptions)
            {
                try
                {
                    await webPushClient.SendNotificationAsync(
                        new PushSubscription(subscription.Endpoint, subscription.P256DH, subscription.Auth),
                        payload,
                        vapidDetails);
                    _logger.LogDebug("User notification sent");
                }
                catch (WebPushException ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        }
    }
}