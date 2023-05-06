using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Configuration;
using BetSnooker.Models;
using BetSnooker.Models.API;
using BetSnooker.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BetSnooker.Services
{
    public class SnookerFeedService : ISnookerFeedService
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly ISnookerCacheService _snookerCacheService;
        private readonly ILogger _logger;

        public SnookerFeedService(ISnookerCacheService snookerCacheService, ISettingsProvider settingsProvider, ILogger<SnookerFeedService> logger)
        {
            _settingsProvider = settingsProvider;
            _snookerCacheService = snookerCacheService;
            _logger = logger;
        }

        public async Task<Event> GetCurrentEvent()
        {
            return await _snookerCacheService.GetEvent(_settingsProvider.EventId);
        }

        public async Task<IEnumerable<RoundInfoDetails>> GetEventRounds()
        {
            int eventId = _settingsProvider.EventId;
            int startRound = _settingsProvider.StartRound;
            var eventRounds = (await _snookerCacheService.GetEventRounds(eventId)).ToList();
            if (!eventRounds.Any())
            {
                _logger.LogError("No event round available");
                return null;
            }

            var validRounds = eventRounds.Where(r => r.EventId == eventId && r.NumMatches > 0); // some events have qualification rounds included

            var eventMatches = (await GetEventMatches(true)).ToList();
            if (!eventMatches.Any())
            {
                _logger.LogError("No event matches available");
                return null;
            }

            var eventMatchesGroupedByRound = eventMatches.GroupBy(m => m.Round).OrderBy(r => r.Key).ToList();

            var validRoundInfoDetails = new List<RoundInfoDetails>();
            foreach (var roundInfo in validRounds)
            {
                try
                {
                    var matchesGrouped = eventMatchesGroupedByRound.Single(r => r.Key == roundInfo.Round);
                    var minScheduledDate = matchesGrouped.Where(m => !m.Walkover1 && !m.Walkover2).Min(m => m.ActualStartDate);
                    var roundFinished = matchesGrouped.All(MatchFinished);

                    var roundInfoDetails = new RoundInfoDetails(roundInfo)
                    {
                        ActualStartDate = minScheduledDate?.ToLocalTime(),
                        Started = minScheduledDate.HasValue && minScheduledDate.Value.ToLocalTime() <= DateTime.Now,
                        Finished = roundFinished
                    };

                    validRoundInfoDetails.Add(roundInfoDetails);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                }
            }

            return validRoundInfoDetails.Where(r => r.Round >= startRound);
        }

        public async Task<RoundInfoDetails> GetCurrentRound(IEnumerable<RoundInfoDetails> rounds)
        {
            if (rounds == null)
            {
                rounds = (await GetEventRounds()).ToList();
            }

            if (!rounds.Any())
            {
                _logger.LogWarning("Could not get current round");
                return null;
            }

            // always return the final if it's current round
            // rounds are ordered by round ID
            RoundInfoDetails currentRound = rounds.FirstOrDefault(eventRound => !eventRound.Finished || eventRound.IsFinalRound);

            return currentRound?.Round >= _settingsProvider.StartRound ? currentRound : null;
        }

        public async Task<RoundInfoDetails> GetNextRound(RoundInfoDetails currentRound)
        {
            var eventRounds = (await GetEventRounds()).ToList();
            if (!eventRounds.Any())
            {
                _logger.LogWarning("Could not get event rounds");
                return null;
            }

            // rounds are ordered by round ID
            return eventRounds.FirstOrDefault(round => round.Round > currentRound.Round);
        }

        public async Task<IEnumerable<MatchDetails>> GetEventMatches(bool allEventMatches = false)
        {
            var eventMatches = await _snookerCacheService.GetEventMatches(_settingsProvider.EventId);
            if (!eventMatches.Any())
            {
                _logger.LogWarning("Could not get any event matches");
                return new List<MatchDetails>();
            }
            
            if (allEventMatches)
            {
                return await ConvertToMatchDetails(eventMatches);
            }

            var startRoundId = _settingsProvider.StartRound;
            var filteredMatches = eventMatches.Where(match => match.Round >= startRoundId).ToList();
            return filteredMatches.Any()
                ? await ConvertToMatchDetails(filteredMatches)
                : new List<MatchDetails>();
        }

        public async Task<IEnumerable<MatchDetails>> GetOngoingMatches()
        {
            IEnumerable<Match> ongoingMatches = await _snookerCacheService.GetOngoingMatches();
            if (ongoingMatches == null || !ongoingMatches.Any())
            {
                return null;
            }

            var eventId = _settingsProvider.EventId;
            var startRoundId = _settingsProvider.StartRound;
            var filteredMatches = ongoingMatches.Where(match => match.EventId == eventId && match.Round >= startRoundId).ToList();
            return filteredMatches.Any()
                ? await ConvertToMatchDetails(filteredMatches)
                : new List<MatchDetails>();
        }

        public async Task<IEnumerable<MatchDetails>> GetRoundMatches(int roundId)
        {
            var eventMatches = await GetEventMatches();
            var roundMatches = eventMatches.Where(m => m.Round == roundId);
            return roundMatches;
        }

        public async Task<IEnumerable<Player>> GetEventPlayers()
        {
            return await _snookerCacheService.GetEventPlayers(_settingsProvider.EventId);
        }

        private async Task<IEnumerable<MatchDetails>> ConvertToMatchDetails(IEnumerable<Match> matches)
        {
            var matchDetailsCollection = new List<MatchDetails>();

            var players = (await GetEventPlayers()).ToList();
            var eventRounds = await GetEventRoundInfos();
            if (!players.Any() || !eventRounds.Any())
            {
                return matchDetailsCollection;
            }
            
            foreach (var match in matches)
            {
                var player1 = players.Single(p => p.Id == match.Player1Id);
                var player2 = players.Single(p => p.Id == match.Player2Id);
                var winner = players.SingleOrDefault(p => p.Id == match.WinnerId);

                var roundInfo = eventRounds.SingleOrDefault(r => r.Round == match.Round);
                if (roundInfo != null)
                {
                    var matchDetails = new MatchDetails(match)
                    {
                        Player1Name = player1.ToString(),
                        Player2Name = player2.ToString(),
                        WinnerName = winner != null ? winner.ToString() : string.Empty,
                        RoundName = roundInfo.RoundName,
                        Distance = roundInfo.Distance
                    };

                    matchDetailsCollection.Add(matchDetails);
                }
            }

            return matchDetailsCollection.AsEnumerable().OrderBy(m => m.Id);
        }

        private async Task<List<RoundInfo>> GetEventRoundInfos()
        {
            var eventRounds = await _snookerCacheService.GetEventRounds(_settingsProvider.EventId);
            var validRounds = eventRounds.Where(r => r.NumMatches > 0);
            return validRounds.ToList();
        }

        private bool MatchFinished(MatchDetails match)
        {
            return !match.Unfinished && (match.Score1 != 0 || match.Score2 != 0 || match.Walkover1 || match.Walkover2); // TODO: WinnerId != 0?
        }
    }
}