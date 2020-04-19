using System;
using System.Collections.Generic;
using System.Linq;
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
            int startRound = _configurationService.StartRound;
            var eventRounds = _snookerHubService.GetEventRounds();
            var validRounds = eventRounds.Where(r => r.NumMatches > 0);
            return allEventRounds ? validRounds : validRounds.Where(r => r.Round >= startRound);
        }

        public RoundInfoDetails GetCurrentRound()
        {
            var eventMatches = GetEventMatches(true);
            var eventMatchesGroupedByRound = eventMatches.GroupBy(m => m.Round).OrderBy(r => r.Key);

            RoundInfoDetails roundInfoDetails = null;
            foreach (var matchesGrouped in eventMatchesGroupedByRound)
            {
                var roundFinished = matchesGrouped.All(match => !match.Unfinished && (match.Score1 != 0 || match.Score2 != 0));
                if (roundFinished && matchesGrouped.Count() > 1) continue; // do not null final round

                var roundId = matchesGrouped.First().Round;
                var roundInfo = GetRoundInfo(roundId);
                
                var minScheduledDate = matchesGrouped.Min(m => m.ScheduledDate);
                roundInfoDetails = new RoundInfoDetails(roundInfo)
                {
                    Started = minScheduledDate.HasValue && minScheduledDate.Value.ToLocalTime() <= DateTime.Now // TODO: local time or UTC?
                };

                break;
            }

            // TODO: dispose SnookerHub if event is finished

            return roundInfoDetails.Round >= _configurationService.StartRound ? roundInfoDetails : null;
        }

        public IEnumerable<MatchDetails> GetEventMatches(bool allEventMatches = false)
        {
            IEnumerable<Match> eventMatches = _snookerHubService.GetEventMatches();
            if (allEventMatches)
            {
                return eventMatches != null && eventMatches.Any()
                    ? ConvertToMatchDetails(eventMatches)
                    : new List<MatchDetails>();
            }

            var startRoundId = _configurationService.StartRound;
            var filteredMatches = eventMatches?.Where(match => match.Round >= startRoundId);

            return filteredMatches != null && filteredMatches.Any()
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
            var players = GetEventPlayers();

            var matchDetailsCollection = new List<MatchDetails>();
            foreach (var match in matches)
            {
                var player1 = players.Single(p => p.Id == match.Player1Id);
                var player2 = players.Single(p => p.Id == match.Player2Id);
                var winner = players.SingleOrDefault(p => p.Id == match.WinnerId);

                var roundInfo = GetRoundInfo(match.Round);
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

        private RoundInfo GetRoundInfo(int roundId)
        {
            var eventRounds = GetEventRounds(true);
            var round = eventRounds.SingleOrDefault(r => r.Round == roundId);
            return new RoundInfoDetails(round);
        }
    }
}