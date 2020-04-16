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
        Task<IEnumerable<RoundInfo>> GetEventRounds(bool allEventRounds = false);
        Task<RoundInfoDetails> GetCurrentRound();

        // matches
        Task<IEnumerable<MatchDetails>> GetEventMatches(bool forceRefresh = true, bool allEventMatches = false);
        Task<IEnumerable<MatchDetails>> GetRoundMatches(int roundId);

        // players
        Task<IEnumerable<Player>> GetEventPlayers();
    }
}