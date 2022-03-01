using System;

namespace BetSnooker.Models
{
    public class NotificationSubscription
    {
        public string Endpoint { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public SubscriptionKeys Keys { get; set; }
    }

    public class SubscriptionKeys
    {
        public string P256DH { get; set; }
        public string Auth { get; set; }
    }
}