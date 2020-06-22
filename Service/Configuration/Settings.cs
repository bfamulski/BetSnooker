using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BetSnooker.Configuration
{
    public class Settings : ISettings
    {
        private readonly string _defaultSnookerApiUrl = "http://api.snooker.org/";
        private readonly int _defaultMaxUsers = 2;
        private readonly TimeSpan _defaultGetMatchesInterval = TimeSpan.FromMinutes(15);
        private readonly ILogger _logger;

        public Settings(IConfiguration configuration, ILogger<Settings> logger)
        {
            _logger = logger;

            GetConfigurationSettings(configuration);
        }

        public int EventId { get; private set; }
        public int StartRound { get; private set; }
        public string SnookerApiUrl { get; private set; }
        public int MaxUsers { get; private set; }
        public TimeSpan GetMatchesInterval { get; private set; }

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

            TimeSpan? getMatchesInterval = null;
            var getMatchesIntervalConfig = configuration["GetMatchesInterval"];
            if (!string.IsNullOrEmpty(getMatchesIntervalConfig))
            {
                if (!TimeSpan.TryParse(getMatchesIntervalConfig, out TimeSpan getMatchesIntervalOutput))
                {
                    throw new ApplicationException("GetMatchesInterval configuration variable is invalid");
                }

                getMatchesInterval = getMatchesIntervalOutput;
            }

            EventId = eventId;
            StartRound = startRound;
            SnookerApiUrl = string.IsNullOrEmpty(snookerApiUrl) ? _defaultSnookerApiUrl : snookerApiUrl;
            MaxUsers = maxUsers ?? _defaultMaxUsers;
            GetMatchesInterval = getMatchesInterval ?? _defaultGetMatchesInterval;

            _logger.LogInformation(
                $"Configuring service with following values - EventId: {EventId}, StartRound: {StartRound}, SnookerApiUrl: {SnookerApiUrl}, MaxUsers: {MaxUsers}, GetMatchesInterval: {GetMatchesInterval}");
        }
    }
}