﻿using System.Collections.Generic;

namespace BetSnooker.Models
{
    public class EventBets
    {
        public string UserId { get; set; }

        public IEnumerable<RoundBets> RoundBets { get; set; }

        public UserScore UserScore { get; set; }
    }
}