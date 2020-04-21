using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BetSnooker.Models.API;
using BetSnooker.Services.Interfaces;

namespace BetSnooker.Services
{
    public class SnookerHubService : ISnookerHubService
    {
        private static readonly int MatchesTimerPeriod = (int)TimeSpan.FromSeconds(30).TotalMilliseconds; // TODO: change it back to 10
        private static readonly int RoundsTimerPeriod = (int)TimeSpan.FromDays(1).TotalMilliseconds;
        private static readonly int PlayersTimerPeriod = (int)TimeSpan.FromDays(1).TotalMilliseconds;
        private static readonly int EventTimerPeriod = (int)TimeSpan.FromDays(1).TotalMilliseconds;

        private static List<Match> _eventMatches = new List<Match>();
        private static List<RoundInfo> _eventRounds = new List<RoundInfo>();
        private static List<Player> _eventPlayers = new List<Player>();
        private static Event _event;

        private readonly Timer _matchesTimer;
        private readonly Timer _roundsTimer;
        private readonly Timer _playersTimer;
        private readonly Timer _eventTimer;
        
        private readonly int _eventId;
        private readonly ISnookerApiService _snookerApiService;

        public SnookerHubService(IConfigurationService configurationService, ISnookerApiService snookerApiService)
        {
            _eventId = configurationService.EventId;
            _snookerApiService = snookerApiService;

            _matchesTimer = new Timer(MatchesTimerEvent, null, 0, MatchesTimerPeriod);
            _roundsTimer = new Timer(RoundsTimerEvent, null, 0, RoundsTimerPeriod);
            _playersTimer = new Timer(PlayersTimerEvent, null, 0, PlayersTimerPeriod);
            _eventTimer = new Timer(EventTimerEvent, null, 0, EventTimerPeriod);
        }

        public Event GetEvent()
        {
            if (_event == null)
            {
                return null;
            }

            lock (_event)
            {
                return _event;
            }
        }

        public IEnumerable<Match> GetEventMatches()
        {
            lock (_eventMatches)
            {
                return _eventMatches;
            }
        }

        public IEnumerable<Player> GetEventPlayers()
        {
            lock (_eventPlayers)
            {
                return _eventPlayers;
            }
        }

        public IEnumerable<RoundInfo> GetEventRounds()
        {
            lock (_eventRounds)
            {
                return _eventRounds;
            }
        }

        public void DisposeHub()
        {
            _matchesTimer?.Dispose();
            _roundsTimer?.Dispose();
            _playersTimer?.Dispose();
            _eventTimer?.Dispose();
        }

        private async void MatchesTimerEvent(object obj)
        {
            var eventMatches = await _snookerApiService.GetEventMatches(_eventId);
            if (eventMatches != null)
            {
                lock (_eventMatches)
                {
                    _eventMatches = eventMatches.ToList();
                }
            }
        }

        private async void RoundsTimerEvent(object obj)
        {
            var eventRounds = await _snookerApiService.GetEventRounds(_eventId);
            if (eventRounds != null)
            {
                lock (_eventRounds)
                {
                    _eventRounds = eventRounds.ToList();
                }
            }
        }

        private async void PlayersTimerEvent(object obj)
        {
            var eventPlayers = await _snookerApiService.GetEventPlayers(_eventId);
            if (eventPlayers != null)
            {
                lock (_eventPlayers)
                {
                    _eventPlayers = eventPlayers.ToList();
                }
            }
        }

        private async void EventTimerEvent(object obj)
        {
            var @event = await _snookerApiService.GetEvent(_eventId);

            if (_event == null)
            {
                _event = @event;
            }
            else
            {
                lock (_event)
                {
                    _event = @event;
                }
            }
        }
    }
}