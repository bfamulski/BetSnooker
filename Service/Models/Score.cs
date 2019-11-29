namespace BetSnooker.Models
{
    public class Score
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int EventId { get; set; }

        public int RoundId { get; set; }

        public int MatchId { get; set; }

        public double ScoreValue { get; set; }
    }
}