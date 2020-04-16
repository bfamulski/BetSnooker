namespace BetSnooker.Services.Interfaces
{
    public interface IConfigurationService
    {
        int EventId { get; }

        int StartRound { get; }

        string SnookerApiUrl { get; }

        int MaxUsers { get; }
    }
}