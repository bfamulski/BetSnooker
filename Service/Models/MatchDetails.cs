using System;
using BetSnooker.Models.API;

namespace BetSnooker.Models
{
    public class MatchDetails : Match
    {
        public MatchDetails(Match match)
        {
            Id = match.Id;
            WorldSnookerId = match.WorldSnookerId;
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
            Unfinished = match.Unfinished;
            OnBreak = match.OnBreak;
            ScheduledDate = match.ScheduledDate;
            StartDate = match.StartDate;
            EndDate = match.EndDate;
            Sessions = match.Sessions;
        }

        public string MatchId => $"{EventId}_{Round}_{Number}";

        public string RoundName { get; set; }

        public int Distance { get; set; }

        public string Player1Name { get; set; }

        public string Player2Name { get; set; }

        public string WinnerName { get; set; }

        public DateTime? ActualStartDate => StartDate ?? ScheduledDate;
    }
}