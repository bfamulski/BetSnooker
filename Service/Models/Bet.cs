namespace BetSnooker.Models
{
    public class Bet
    {
        public int Id { get; set; }

        public int MatchId { get; set; }

        public int Player1Id { get; set; }

        public string Player1Name { get; set; }

        public int? Score1 { get; set; }

        public int Player2Id { get; set; }

        public string Player2Name { get; set; }

        public int? Score2 { get; set; }

        //public int RoundBetsID { get; set; }
        //public RoundBets RoundBets { get; set; }
    }
}