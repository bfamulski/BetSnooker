using System;

namespace BetSnooker.Models
{
    public class UserSubscription
    {
        public int Id { get; set; }
        public string Endpoint { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public string P256DH { get; set; }
        public string Auth { get; set; }
    }
}