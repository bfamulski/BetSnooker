using BetSnooker.Models.API;

namespace BetSnooker.Models
{
    public class MatchDetails : Match
    {
        public MatchDetails(Match match)
        {
            Id = match.Id;
            MatchId = match.WorldSnookerId;
            EventId = match.EventId;
            Round = match.Round;
            Number = match.Number;
            Player1Id = match.Player1Id;
            Score1 = match.Score1;
            Walkover1 = match.Walkover1;
            Player2Id = match.Player2Id;
            Score2 = match.Score2;
            Walkover2 = match.Walkover2;
            WinnerId = match.WinnerId;
            ScheduledDate = match.ScheduledDate;
            Unfinished = match.Unfinished;
        }

        public int MatchId { get; set; }

        public string RoundName { get; set; }

        public string Player1Name { get; set; }

        public string Player2Name { get; set; }

        public string WinnerName { get; set; }
    }
}