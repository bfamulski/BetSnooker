using System;
using BetSnooker.Models.API;

namespace BetSnooker.Models
{
    public class RoundInfoDetails : RoundInfo
    {
        public RoundInfoDetails(RoundInfo roundInfo)
        {
            Round = roundInfo.Round;
            RoundName = roundInfo.RoundName;
            EventId = roundInfo.EventId;
            Distance = roundInfo.Distance;
            NumMatches = roundInfo.NumMatches;
        }

        public DateTime? ActualStartDate { get; set; }

        public bool Started { get; set; }

        public bool Finished { get; set; }

        public bool IsFinalRound => NumMatches == 1;
    }
}