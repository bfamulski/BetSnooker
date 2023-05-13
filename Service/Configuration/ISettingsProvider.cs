namespace BetSnooker.Configuration
{
    public interface ISettingsProvider
    {
        string SnookerApiUrl { get; }
        int EventId { get; }
        int StartRound { get; }
        string RequestedByHeader { get; }
        int UnknownPlayerId { get; }
        int MaxUsers { get; }
    }
}