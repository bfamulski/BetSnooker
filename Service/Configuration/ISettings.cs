using System;

namespace BetSnooker.Configuration
{
    public interface ISettings
    {
        int EventId { get; }
        int StartRound { get; }
        string SnookerApiUrl { get; }
        int MaxUsers { get; }
        TimeSpan GetMatchesInterval { get; }
    }
}