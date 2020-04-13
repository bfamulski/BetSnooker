namespace BetSnooker.Models
{
    public class Bet
    {
        public int Id { get; set; }

        public string MatchId { get; set; }

        public int Player1Id { get; set; }

        public string Player1Name { get; set; }

        public int? Score1 { get; set; }

        public int Player2Id { get; set; }

        public string Player2Name { get; set; }

        public int? Score2 { get; set; }

        public int? WinnerId =>
            Score1.HasValue && Score2.HasValue
                ? (int?)(Score1.Value > Score2.Value ? Player1Id : Player2Id)
                : null;

        public double? ScoreValue { get; set; }

        public int? Error { get; set; }
    }
}