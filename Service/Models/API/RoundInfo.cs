namespace BetSnooker.Models.API
{
    public class RoundInfo
    {
        public int Round { get; set; }

        public string RoundName { get; set; }

        public int EventId { get; set; }

        public int Distance { get; set; }

        public int NumMatches { get; set; }
    }
}