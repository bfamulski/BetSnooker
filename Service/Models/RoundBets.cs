using System;
using System.Collections.Generic;

namespace BetSnooker.Models
{
    public class RoundBets
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int EventId { get; set; }

        public int RoundId { get; set; }

        public int Distance { get; set; }

        public ICollection<Bet> MatchBets { get; set; } = new List<Bet>();
    }
}