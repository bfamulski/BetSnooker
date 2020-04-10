using System;

namespace BetSnooker.Models.API
{
    public class Match
    {
        public int Id { get; set; } // TODO: this ID changes with requests

        public int WorldSnookerId { get; set; }

        public int EventId { get; set; }

        public int Round { get; set; }

        public int Number { get; set; }

        public int Player1Id { get; set; }

        public int Score1 { get; set; }

        public bool Walkover1 { get; set; }

        public int Player2Id { get; set; }

        public int Score2 { get; set; }

        public bool Walkover2 { get; set; }

        public int WinnerId { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public bool Unfinished { get; set; }

        public string MatchId => $"{EventId}_{Round}_{Number}";
    }
}