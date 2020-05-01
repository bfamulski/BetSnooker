using System;

namespace BetSnooker.Configuration
{
    public class Settings
    {
        public int EventId { get; set; }

        public int StartRound { get; set; }

        public string SnookerApiUrl { get; set; }

        public int MaxUsers { get; set; }

        public TimeSpan GetMatchesInterval { get; set; }
    }
}