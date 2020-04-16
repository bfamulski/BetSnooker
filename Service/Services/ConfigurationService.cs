using BetSnooker.Services.Interfaces;

namespace BetSnooker.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private string _defaultSnookerApiUrl = "http://api.snooker.org/";
        private int _defaultMaxUsers = 2;

        public ConfigurationService(int eventId, int startRound, string snookerApiUrl = null, int? maxUsers = null)
        {
            EventId = eventId;
            StartRound = startRound;
            SnookerApiUrl = string.IsNullOrEmpty(snookerApiUrl) ? _defaultSnookerApiUrl : snookerApiUrl;
            MaxUsers = maxUsers ?? _defaultMaxUsers;
        }

        public int EventId { get; }

        public int StartRound { get; }
        
        public string SnookerApiUrl { get; }
        
        public int MaxUsers { get; }
    }
}