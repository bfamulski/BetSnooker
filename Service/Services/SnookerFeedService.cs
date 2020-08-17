using System;
using System.Collections.Generic;
using System.Linq;
using BetSnooker.Configuration;
using BetSnooker.Models;
using BetSnooker.Models.API;
using BetSnooker.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BetSnooker.Services
{
    public class SnookerFeedService : ISnookerFeedService
    {
        private readonly ISettings _settings;
        private readonly ISnookerHubService _snookerHubService;
        private readonly ILogger _logger;

        public SnookerFeedService(ISnookerHubService snookerHubService, ISettings settings, ILogger<SnookerFeedService> logger)
        {
            _snookerHubService = snookerHubService;
            _settings = settings;
            _logger = logger;
        }

        public Event GetCurrentEvent()
        {
            return _snookerHubService.GetEvent();
        }

        public IEnumerable<RoundInfoDetails> GetEventRounds()
        {
            int eventId = _settings.EventId;
            int startRound = _settings.StartRound;
            var eventRounds = _snookerHubService.GetEventRounds().ToList();
            if (!eventRounds.Any())
            {
                _logger.LogError("No event round available");
                return null;
            }

            var validRounds = eventRounds.Where(r => r.EventId == eventId && r.NumMatches > 0); // some events have qualification rounds included

            var eventMatches = GetEventMatches(true).ToList();
            if (!eventMatches.Any())
            {
                _logger.LogError("No event matches available");
                return null;
            }

            var eventMatchesGroupedByRound = eventMatches.GroupBy(m => m.Round).OrderBy(r => r.Key).ToList();

            var validRoundInfoDetails = new List<RoundInfoDetails>();
            foreach (var roundInfo in validRounds)
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

            return validRoundInfoDetails.Where(r => r.Round >= startRound);
        }

        public RoundInfoDetails GetCurrentRound(IEnumerable<RoundInfoDetails> rounds)
        {
            if (rounds == null)
            {
                rounds = GetEventRounds().ToList();
            }

            if (!rounds.Any())
            {
                _logger.LogWarning("Could not get current round");
                return null;
            }

            // always return the final if it's current round
            // rounds are ordered by round ID
            RoundInfoDetails currentRound = rounds.FirstOrDefault(eventRound => !eventRound.Finished || eventRound.IsFinalRound);

            if (currentRound != null && currentRound.IsFinalRound && currentRound.Finished)
            {
                _logger.LogInformation("Event finished. Disposing Snooker Hub");
                _snookerHubService.DisposeHub();
            }

            return currentRound?.Round >= _settings.StartRound ? currentRound : null;
        }

        public RoundInfoDetails GetNextRound(RoundInfoDetails currentRound)
        {
            var eventRounds = GetEventRounds().ToList();
            if (!eventRounds.Any())
            {
                _logger.LogWarning("Could not get event rounds");
                return null;
            }

            // rounds are ordered by round ID
            return eventRounds.FirstOrDefault(round => round.Round > currentRound.Round);
        }

        public IEnumerable<MatchDetails> GetEventMatches(bool allEventMatches = false)
        {
            IEnumerable<Match> eventMatches = _snookerHubService.GetEventMatches().ToList();
            if (!eventMatches.Any())
            {
                _logger.LogWarning("Could not get any event matches");
                return new List<MatchDetails>();
            }
            
            if (allEventMatches)
            {
                return ConvertToMatchDetails(eventMatches);
            }

            var startRoundId = _settings.StartRound;
            var filteredMatches = eventMatches.Where(match => match.Round >= startRoundId).ToList();
            return filteredMatches.Any()
                ? ConvertToMatchDetails(filteredMatches)
                : new List<MatchDetails>();
        }

        public IEnumerable<MatchDetails> GetRoundMatches(int roundId)
        {
            var eventMatches = GetEventMatches();
            var roundMatches = eventMatches.Where(m => m.Round == roundId);
            return ConvertToMatchDetails(roundMatches);
        }

        public IEnumerable<Player> GetEventPlayers()
        {
            return _snookerHubService.GetEventPlayers();
        }

        private IEnumerable<MatchDetails> ConvertToMatchDetails(IEnumerable<Match> matches)
        {
            var matchDetailsCollection = new List<MatchDetails>();

            var players = GetEventPlayers().ToList();
            var eventRounds = GetEventRoundInfos();
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

            return matchDetailsCollection.AsEnumerable().OrderBy(m => m.Id);
        }

        private List<RoundInfo> GetEventRoundInfos()
        {
            int startRound = _settings.StartRound;
            var eventRounds = _snookerHubService.GetEventRounds();
            var validRounds = eventRounds.Where(r => r.NumMatches > 0);
            return validRounds.Where(r => r.Round >= startRound).ToList();
        }

        private bool MatchFinished(MatchDetails match)
        {
            return !match.Unfinished && (match.Score1 != 0 || match.Score2 != 0 || match.Walkover1 || match.Walkover2); // TODO: WinnerId != 0?
        }
    }
}