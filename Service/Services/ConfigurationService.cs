using BetSnooker.Services.Interfaces;

namespace BetSnooker.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public ConfigurationService(int eventId, int startRound, string snookerApiUrl)
        {
            EventId = eventId;
            StartRound = startRound;
            SnookerApiUrl = snookerApiUrl;
        }

        public int EventId { get; }

        public int StartRound { get; }
        
        public string SnookerApiUrl { get; }
    }
}