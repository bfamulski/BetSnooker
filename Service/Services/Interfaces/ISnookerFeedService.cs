using System.Collections.Generic;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Models.API;

namespace BetSnooker.Services.Interfaces
{
    public interface ISnookerFeedService
    {
        // events
        Event GetCurrentEvent();

        // rounds
        IEnumerable<RoundInfoDetails> GetEventRounds();
        RoundInfoDetails GetCurrentRound(IEnumerable<RoundInfoDetails> rounds);
        RoundInfoDetails GetNextRound(RoundInfoDetails currentRound);

        // matches
        IEnumerable<MatchDetails> GetEventMatches(bool allEventMatches = false);
        IEnumerable<MatchDetails> GetRoundMatches(int roundId);
        Task<IEnumerable<MatchDetails>> GetOngoingMatches();

        // players
        IEnumerable<Player> GetEventPlayers();
    }
}