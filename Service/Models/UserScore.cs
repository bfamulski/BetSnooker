namespace BetSnooker.Models
{
    public class UserScore
    {
        public int MatchesFinished { get; set; }

        public double? EventScore { get; set; }

        public int CorrectWinners { get; set; }

        public int ExactScores { get; set; }

        public double CorrectWinnersAccuracy { get; set; }

        public double ExactScoresAccuracy { get; set; }

        public double? AverageError { get; set; }

        public bool IsWinner { get; set; }
    }
}
