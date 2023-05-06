using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BetSnooker.Configuration
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly string _defaultSnookerApiUrl = "https://api.snooker.org/";
        private readonly int _defaultMaxUsers = 4;
        private readonly string _defaultRequestedByHeader = "";
        private readonly ILogger _logger;

        public SettingsProvider(IConfiguration configuration, ILogger<SettingsProvider> logger)
        {
            _logger = logger;
            GetConfigurationSettings(configuration);
        }

        public int EventId { get; private set; }
        public int StartRound { get; private set; }
        public string SnookerApiUrl { get; private set; }
        public int MaxUsers { get; private set; }
        public string RequestedByHeader { get; private set; }

        private void GetConfigurationSettings(IConfiguration configuration)
        {
            var eventIdConfig = configuration["EventID"];
            var startRoundConfig = configuration["StartRound"];
            if (string.IsNullOrEmpty(eventIdConfig) || string.IsNullOrEmpty(startRoundConfig))
            {
                throw new ApplicationException("EventID and/or StartRound configuration variable is not set");
            }

            if (!int.TryParse(eventIdConfig, out int eventId) || !int.TryParse(startRoundConfig, out int startRound))
            {
                throw new ApplicationException("EventID and/or StartRound configuration variable is invalid");
            }

            var snookerApiUrl = configuration["SnookerApiUrl"];

            int? maxUsers = null;
            var maxUsersConfig = configuration["MaxUsers"];
            if (!string.IsNullOrEmpty(maxUsersConfig))
            {
                if (!int.TryParse(maxUsersConfig, out int maxUsersOutput))
                {
                    throw new ApplicationException("MaxUsers configuration variable is invalid");
                }

                maxUsers = maxUsersOutput;
            }

            var requestedByHeader = configuration["RequestedByHeader"];

            EventId = eventId;
            StartRound = startRound;
            SnookerApiUrl = string.IsNullOrEmpty(snookerApiUrl) ? _defaultSnookerApiUrl : snookerApiUrl;
            MaxUsers = maxUsers ?? _defaultMaxUsers;
            RequestedByHeader = string.IsNullOrEmpty(requestedByHeader) ? _defaultRequestedByHeader : requestedByHeader;

            _logger.LogInformation(
                $"Configuring service with following values - EventId: {EventId}, StartRound: {StartRound}, SnookerApiUrl: {SnookerApiUrl}, MaxUsers: {MaxUsers}, RequestedByHeader: {RequestedByHeader}");
        }
    }
}