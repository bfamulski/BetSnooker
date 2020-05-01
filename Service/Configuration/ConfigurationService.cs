using System;
using Microsoft.Extensions.Configuration;

namespace BetSnooker.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private string _defaultSnookerApiUrl = "http://api.snooker.org/";
        private int _defaultMaxUsers = 2;
        private readonly TimeSpan _defaultGetMatchesInterval = TimeSpan.FromMinutes(15);

        public Settings Settings { get; }

        public ConfigurationService(IConfiguration configuration)
        {
            Settings = GetConfigurationSettings(configuration);
        }

        private Settings GetConfigurationSettings(IConfiguration configuration)
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

            return new Settings
            {
                EventId = eventId,
                StartRound = startRound,
                SnookerApiUrl = string.IsNullOrEmpty(snookerApiUrl) ? _defaultSnookerApiUrl : snookerApiUrl,
                MaxUsers = maxUsers ?? _defaultMaxUsers,
                GetMatchesInterval = getMatchesInterval ?? _defaultGetMatchesInterval
            };
        }
    }
}