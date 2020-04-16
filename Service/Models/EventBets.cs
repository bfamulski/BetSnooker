using System.Collections.Generic;

namespace BetSnooker.Models
{
    public class EventBets
    {
        public string UserId { get; set; }

        public IEnumerable<RoundBets> RoundBets { get; set; }

        public int MatchesFinished { get; set; }

        public double? EventScore { get; set; }

        public int CorrectWinners { get; set; }
        
        public int ExactScores { get; set; }

        public double CorrectWinnersAccuracy { get; set; }

        public double ExactScoresAccuracy { get; set; }
    }
}