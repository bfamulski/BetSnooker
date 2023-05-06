using System.Collections.Generic;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Models.API;

namespace BetSnooker.Services.Interfaces
{
    public interface ISnookerFeedService
    {
        // events
        Task<Event> GetCurrentEvent();

        // rounds
        Task<IEnumerable<RoundInfoDetails>> GetEventRounds();
        Task<RoundInfoDetails> GetCurrentRound(IEnumerable<RoundInfoDetails> rounds);
        Task<RoundInfoDetails> GetNextRound(RoundInfoDetails currentRound);

        // matches
        Task<IEnumerable<MatchDetails>> GetEventMatches(bool allEventMatches = false);
        Task<IEnumerable<MatchDetails>> GetRoundMatches(int roundId);
        Task<IEnumerable<MatchDetails>> GetOngoingMatches();

        // players
        Task<IEnumerable<Player>> GetEventPlayers();
    }
}