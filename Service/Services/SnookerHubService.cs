using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BetSnooker.Configuration;
using BetSnooker.Models.API;
using BetSnooker.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BetSnooker.Services
{
    public class SnookerHubService : ISnookerHubService
    {
        private static readonly TimeSpan GetRoundsTimerPeriod = TimeSpan.FromDays(1);
        private static readonly TimeSpan GetPlayersTimerPeriod = TimeSpan.FromDays(1);
        private static readonly TimeSpan GetEventTimerPeriod = TimeSpan.FromDays(1);

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
        private readonly ILogger _logger;

        public SnookerHubService(ISnookerApiService snookerApiService, ISettings settings, ILogger<SnookerHubService> logger)
        {
            _eventId = settings.EventId;
            _snookerApiService = snookerApiService;
            _logger = logger;

            _matchesTimer = new Timer(GetMatchesTimerEvent, null, TimeSpan.Zero, settings.GetMatchesInterval);
            _roundsTimer = new Timer(GetRoundsTimerEvent, null, TimeSpan.Zero, GetRoundsTimerPeriod);
            _playersTimer = new Timer(GetPlayersTimerEvent, null, TimeSpan.Zero, GetPlayersTimerPeriod);
            _eventTimer = new Timer(GetEventTimerEvent, null, TimeSpan.Zero, GetEventTimerPeriod);
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
            _logger.LogInformation("Disposing Snooker Hub");
            _matchesTimer?.Dispose();
            _roundsTimer?.Dispose();
            _playersTimer?.Dispose();
            _eventTimer?.Dispose();
            _logger.LogInformation("Snooker Hub disposed");
        }

        private async void GetMatchesTimerEvent(object obj)
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

        private async void GetRoundsTimerEvent(object obj)
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

        private async void GetPlayersTimerEvent(object obj)
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

        private async void GetEventTimerEvent(object obj)
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