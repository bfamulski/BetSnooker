using System;

namespace BetSnooker.Models.API
{
    public class Event
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Season { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}