using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using BetSnooker.Models;

namespace BetSnooker.Services
{
    public interface IStateService
    {
    }

    public class StateService : IStateService
    {
        private const int TimerIntervalMs = 5 * 60 * 1000; // 5 mins
        private readonly Timer _timer;

        private const int EventTimerIntervalMs =/* 60 * 60 **/ 10000; // 1 hour
        private readonly Timer _eventTimer;

        private readonly ISnookerFeedService _snookerFeedService;

        public StateService(ISnookerFeedService snookerFeedService)
        {
            _snookerFeedService = snookerFeedService;

            _eventTimer = new Timer(EventTimerIntervalMs);
            _eventTimer.Elapsed += EventTimerOnElapsed;
            //_eventTimer.Start();

            //_timer = new Timer(TimerIntervalMs);
            //_timer.Elapsed += TimerOnElapsed;
            //_timer.Start();
        }

        private async void EventTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            var matchesOfRound = await _snookerFeedService.GetEventMatches();

            const int tbdId = 666;

            var matchesGroupedByRound = matchesOfRound.GroupBy(m => m.Round);
            foreach (var grouping in matchesGroupedByRound)
            {
                var roundStart = grouping.Min(m => m.ScheduledDate);

                bool roundReady = true;
                foreach (var match in grouping)
                {
                    if (match.Player1Id == tbdId || match.Player2Id == tbdId)
                    {
                        roundReady = false;
                    }
                }
            }
        }

        private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            var matchesOfRound = await _snookerFeedService.GetRoundMatches(12);
            var roundStart = matchesOfRound.Min(m => m.ScheduledDate);

            if (roundStart >= DateTime.UtcNow.AddMinutes(5))
            {
                //foreach (var bet in InMemoryStore.UserRoundBets)
                //{
                //    // send a bet to a database
                //}

                _timer.Stop();
            }
        }
    }
}