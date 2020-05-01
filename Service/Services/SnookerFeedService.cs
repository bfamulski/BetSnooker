using System;
using System.Collections.Generic;
using System.Linq;
using BetSnooker.Configuration;
using BetSnooker.Models;
using BetSnooker.Models.API;
using BetSnooker.Services.Interfaces;

namespace BetSnooker.Services
{
    public class SnookerFeedService : ISnookerFeedService
    {
        private readonly IConfigurationService _configurationService;
        private readonly ISnookerHubService _snookerHubService;

        public SnookerFeedService(IConfigurationService configurationService, ISnookerHubService snookerHubService)
        {
            _configurationService = configurationService;
            _snookerHubService = snookerHubService;
        }

        public Event GetCurrentEvent()
        {
            return _snookerHubService.GetEvent();
        }

        public IEnumerable<RoundInfo> GetEventRounds(bool allEventRounds = false)
        {
            int startRound = _configurationService.Settings.StartRound;
            var eventRounds = _snookerHubService.GetEventRounds();
            var validRounds = eventRounds.Where(r => r.NumMatches > 0);
            return allEventRounds ? validRounds : validRounds.Where(r => r.Round >= startRound);
        }

        public RoundInfoDetails GetCurrentRound()
        {
            var eventRounds = GetEventRounds(true).ToList();
            var eventMatches = GetEventMatches(true).ToList();
            if (!eventRounds.Any() || !eventMatches.Any())
            {
                return null;
            }

            RoundInfoDetails roundInfoDetails = null;
            var eventMatchesGroupedByRound = eventMatches.GroupBy(m => m.Round).OrderBy(r => r.Key);
            foreach (var matchesGrouped in eventMatchesGroupedByRound)
            {
                var roundFinished = matchesGrouped.All(MatchFinished);
                if (roundFinished && matchesGrouped.Count() > 1) continue; // always return the final if it's current round

                var roundId = matchesGrouped.Key; // First().Round;
                var roundInfo = GetRoundInfo(roundId, eventRounds);

                var minScheduledDate = matchesGrouped.Min(m => m.ScheduledDate);
                roundInfoDetails = new RoundInfoDetails(roundInfo)
                {
                    Started = minScheduledDate.HasValue && minScheduledDate.Value.ToLocalTime() <= DateTime.Now, // TODO: local time or UTC?
                    Finished = roundFinished
                };

                break;
            }

            if (roundInfoDetails != null && roundInfoDetails.IsFinalRound && roundInfoDetails.Finished)
            {
                _snookerHubService.DisposeHub();
            }

            return roundInfoDetails?.Round >= _configurationService.Settings.StartRound ? roundInfoDetails : null;
        }

        public IEnumerable<MatchDetails> GetEventMatches(bool allEventMatches = false)
        {
            IEnumerable<Match> eventMatches = _snookerHubService.GetEventMatches().ToList();
            if (!eventMatches.Any())
            {
                return new List<MatchDetails>();
            }
            
            if (allEventMatches)
            {
                return ConvertToMatchDetails(eventMatches);
            }

            var startRoundId = _configurationService.Settings.StartRound;
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
            var eventRounds = GetEventRounds().ToList();
            if (!players.Any() || !eventRounds.Any())
            {
                return matchDetailsCollection;
            }
            
            foreach (var match in matches)
            {
                var player1 = players.Single(p => p.Id == match.Player1Id);
                var player2 = players.Single(p => p.Id == match.Player2Id);
                var winner = players.SingleOrDefault(p => p.Id == match.WinnerId);

                var roundInfo = GetRoundInfo(match.Round, eventRounds);
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

        private RoundInfo GetRoundInfo(int roundId, IEnumerable<RoundInfo> eventRounds)
        {
            return eventRounds.SingleOrDefault(r => r.Round == roundId);
        }

        private bool MatchFinished(MatchDetails match)
        {
            return !match.Unfinished && (match.Score1 != 0 || match.Score2 != 0 || match.Walkover1 || match.Walkover2); // TODO: WinnerId != 0?
        }
    }
}