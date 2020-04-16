using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Models.API;
using BetSnooker.Services.Interfaces;

namespace BetSnooker.Services
{
    public class SnookerFeedService : ISnookerFeedService
    {
        // TODO: implement better caching
        private readonly ICacheService<RoundInfo> _cachedRounds = new CacheService<RoundInfo>(TimeSpan.FromDays(1));
        private readonly ICacheService<Player> _cachedPlayers = new CacheService<Player>(TimeSpan.FromDays(1));

        private static readonly IDictionary<int, List<Match>> _cachedMatches = new ConcurrentDictionary<int, List<Match>>();

        private readonly ISnookerApiService _snookerApiService;
        private readonly IConfigurationService _configurationService;

        public SnookerFeedService(ISnookerApiService snookerApiService, IConfigurationService configurationService)
        {
            _snookerApiService = snookerApiService;
            _configurationService = configurationService;
        }

        public async Task<Event> GetCurrentEvent()
        {
            return await _snookerApiService.GetEvent(_configurationService.EventId);
        }

        public async Task<IEnumerable<RoundInfo>> GetEventRounds(bool allEventRounds = false)
        {
            int eventId = _configurationService.EventId;
            int startRound = _configurationService.StartRound;

            if (_cachedRounds.Exists())
            {
                var rounds = _cachedRounds.Get();
                return allEventRounds ? rounds : rounds.Where(r => r.Round >= startRound);
            }

            var eventRounds = await _snookerApiService.GetEventRounds(eventId);
            var filteredRounds = eventRounds.Where(r => r.NumMatches > 0);
            _cachedRounds.Save(filteredRounds.ToList());

            return allEventRounds ? filteredRounds : filteredRounds.Where(r => r.Round >= startRound);
        }

        public async Task<RoundInfoDetails> GetCurrentRound()
        {
            var eventMatches = await GetEventMatches(false, true);
            var eventMatchesGroupedByRound = eventMatches.GroupBy(m => m.Round).OrderBy(r => r.Key);

            RoundInfoDetails roundInfoDetails = null;
            foreach (var matchesGrouped in eventMatchesGroupedByRound)
            {
                var roundFinished = matchesGrouped.All(match => !match.Unfinished && (match.Score1 != 0 || match.Score2 != 0));
                if (roundFinished && matchesGrouped.Count() > 1) continue; // do not null final round

                var roundId = matchesGrouped.First().Round;
                var roundInfo = await GetRoundInfo(roundId);
                
                var minScheduledDate = matchesGrouped.Min(m => m.ScheduledDate);
                roundInfoDetails = new RoundInfoDetails(roundInfo)
                {
                    Started = minScheduledDate.HasValue && minScheduledDate.Value.ToLocalTime() <= DateTime.Now
                };

                break;
            }

            return roundInfoDetails.Round >= _configurationService.StartRound ? roundInfoDetails : null;
        }

        public async Task<IEnumerable<MatchDetails>> GetEventMatches(bool forceRefresh = true, bool allEventMatches = false)
        {
            int eventId = _configurationService.EventId;
            IEnumerable<Match> eventMatches;
            if (forceRefresh)
            {
                eventMatches = await _snookerApiService.GetEventMatches(eventId);
                if (eventMatches != null && eventMatches.Any())
                {
                    lock (_cachedMatches)
                    {
                        _cachedMatches[eventId] = eventMatches.ToList();
                    }
                }
            }
            else
            {
                bool cacheContainsKey;
                lock (_cachedMatches)
                {
                    cacheContainsKey = _cachedMatches.ContainsKey(eventId);
                }

                if (cacheContainsKey)
                {
                    lock (_cachedMatches)
                    {
                        eventMatches = _cachedMatches[eventId];
                    }
                }
                else
                {
                    eventMatches = await _snookerApiService.GetEventMatches(eventId);
                    if (eventMatches != null && eventMatches.Any())
                    {
                        lock (_cachedMatches)
                        {
                            _cachedMatches[eventId] = eventMatches.ToList();
                        }
                    }
                }
            }

            if (allEventMatches)
            {
                return eventMatches != null && eventMatches.Any()
                    ? await ConvertToMatchDetails(eventMatches)
                    : new List<MatchDetails>();
            }

            var startRoundId = _configurationService.StartRound;
            var filteredMatches = eventMatches?.Where(match => match.Round >= startRoundId);

            return filteredMatches != null && filteredMatches.Any()
                ? await ConvertToMatchDetails(filteredMatches)
                : new List<MatchDetails>();
        }

        public async Task<IEnumerable<MatchDetails>> GetRoundMatches(int roundId)
        {
            var eventMatches = await GetEventMatches();
            var roundMatches = eventMatches.Where(m => m.Round == roundId);
            return await ConvertToMatchDetails(roundMatches);
        }

        public async Task<IEnumerable<Player>> GetEventPlayers()
        {
            int eventId = _configurationService.EventId;

            if (_cachedPlayers.Exists())
            {
                return _cachedPlayers.Get();
            }

            var eventPlayers = await _snookerApiService.GetEventPlayers(eventId);
            _cachedPlayers.Save(eventPlayers.ToList());

            return eventPlayers;
        }

        private async Task<IEnumerable<MatchDetails>> ConvertToMatchDetails(IEnumerable<Match> matches)
        {
            var players = await GetEventPlayers();

            var matchDetailsCollection = new List<MatchDetails>();
            foreach (var match in matches)
            {
                var player1 = players.Single(p => p.Id == match.Player1Id);
                var player2 = players.Single(p => p.Id == match.Player2Id);
                var winner = players.SingleOrDefault(p => p.Id == match.WinnerId);

                var roundInfo = await GetRoundInfo(match.Round);
                var matchDetails = new MatchDetails(match)
                {
                    Player1Name = player1.ToString(),
                    Player2Name = player2.ToString(),
                    WinnerName = winner != null ? winner.ToString() : "",
                    RoundName = roundInfo.RoundName,
                    Distance = roundInfo.Distance
                };

                matchDetailsCollection.Add(matchDetails);
            }

            return matchDetailsCollection.AsEnumerable().OrderBy(m => m.Id);
        }

        private async Task<RoundInfo> GetRoundInfo(int roundId)
        {
            var eventRounds = await GetEventRounds(true);
            var round = eventRounds.SingleOrDefault(r => r.Round == roundId);
            return new RoundInfoDetails(round);
        }
    }
}