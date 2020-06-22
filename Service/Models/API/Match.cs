using System;

namespace BetSnooker.Models.API
{
    public class Match
    {
        public int Id { get; set; } // TODO: this ID changes with requests
        public int WorldSnookerId { get; set; } // TODO: this is probably useless (verify on live event)
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
        public bool Unfinished { get; set; }
        public bool OnBreak { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Sessions { get; set; }
    }
}