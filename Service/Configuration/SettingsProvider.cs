using System;
using Microsoft.Extensions.Logging;

namespace BetSnooker.Configuration
{
    public class SettingsProvider : ISettingsProvider
    {
        private const string _defaultSnookerApiUrl = "http://api.snooker.org/";
        private const int _defaultMaxUsers = 2;
        private readonly TimeSpan _defaultGetMatchesInterval = TimeSpan.FromMinutes(15);

        private const string EventIdVariableName = "EventID";
        private const string StartRoundVariableName = "StartRound";
        private const string SnookerApiUrlVariableName = "SnookerApiUrl";
        private const string MaxUsersVariableName = "MaxUsers";
        private const string GetMatchesIntervalVariableName = "GetMatchesInterval";

        private readonly ILogger _logger;

        public SettingsProvider(ILogger<SettingsProvider> logger)
        {
            _logger = logger;

            var eventId = GetEnvironmentVariableValue(EventIdVariableName, true);
            EventId = int.Parse(eventId);

            var startRound = GetEnvironmentVariableValue(StartRoundVariableName, true);
            StartRound = int.Parse(startRound);

            var snookerApiUrl = GetEnvironmentVariableValue(SnookerApiUrlVariableName);
            SnookerApiUrl = string.IsNullOrEmpty(snookerApiUrl) ? _defaultSnookerApiUrl : snookerApiUrl;

            var maxUsers = GetEnvironmentVariableValue(MaxUsersVariableName);
            MaxUsers = string.IsNullOrEmpty(maxUsers) ? _defaultMaxUsers : int.Parse(maxUsers);

            TimeSpan? getMatchesInterval = null;
            var getMatchesIntervalConfig = GetEnvironmentVariableValue(GetMatchesIntervalVariableName);
            if (!string.IsNullOrEmpty(getMatchesIntervalConfig))
            {
                if (!TimeSpan.TryParse(getMatchesIntervalConfig, out TimeSpan getMatchesIntervalOutput))
                {
                    throw new ApplicationException($"GetMatchesInterval variable is invalid: {getMatchesIntervalConfig}");
                }

                getMatchesInterval = getMatchesIntervalOutput;
            }

            GetMatchesInterval = getMatchesInterval ?? _defaultGetMatchesInterval;

            _logger.LogInformation(
                $"Configuring service with following values - EventId: {EventId}, StartRound: {StartRound}, SnookerApiUrl: {SnookerApiUrl}, MaxUsers: {MaxUsers}, GetMatchesInterval: {GetMatchesInterval}");
        }

        public int EventId { get; private set; }
        public int StartRound { get; private set; }
        public string SnookerApiUrl { get; private set; }
        public int MaxUsers { get; private set; }
        public TimeSpan GetMatchesInterval { get; private set; }

        private string GetEnvironmentVariableValue(string variableName, bool throwIfNotExist = false)
        {
            var variableValue = Environment.GetEnvironmentVariable(variableName);
            if (throwIfNotExist && string.IsNullOrEmpty(variableValue))
            {
                throw new ApplicationException($"{variableName} variable is not provided");
            }

            return variableValue;
        }
    }
}