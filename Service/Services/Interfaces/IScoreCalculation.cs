using System.Collections.Generic;
using BetSnooker.Models;

namespace BetSnooker.Services.Interfaces
{
    public interface IScoreCalculation
    {
        IEnumerable<EventBets> CalculateAllScores(IEnumerable<RoundBets> eventBets, IEnumerable<MatchDetails> eventMatches, RoundInfoDetails currentRound);
    }
}
