using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Models.API;
using BetSnooker.Repositories.Interfaces;
using BetSnooker.Services.Interfaces;

namespace BetSnooker.Services
{
    public class BetsService : IBetsService
    {
        private readonly IBetsRepository _betsRepository;
        private readonly ISnookerFeedService _snookerFeedService;
        private readonly IConfigurationService _configurationService;

        public BetsService(IBetsRepository betsRepository, ISnookerFeedService snookerFeedService, IConfigurationService configurationService)
        {
            _betsRepository = betsRepository;
            _snookerFeedService = snookerFeedService;
            _configurationService = configurationService;
        }

        public async Task<IEnumerable<RoundBets>> GetAllBets()
        {
            var eventRounds = await _snookerFeedService.GetEventRounds();
            var currentRound = await _snookerFeedService.GetCurrentRound();
            if (currentRound == null)
            {
                return null;
            }
            
            var filteredRounds = currentRound.Started
                ? eventRounds.Where(r => r.Round <= currentRound.Round)
                : eventRounds.Where(r => r.Round < currentRound.Round);

            var filteredBets = await Task.Run(() => _betsRepository.GetAllBets(filteredRounds.Select(r => r.Round).ToArray()));
            if (filteredBets == null || !filteredBets.Any())
            {
                return null;
            }

            var eventMatches = await _snookerFeedService.GetEventMatches();
            foreach (var bet in filteredBets)
            {
                foreach (var matchBet in bet.MatchBets)
                {
                    var eventMatch = eventMatches.Single(m => m.MatchId == matchBet.MatchId);
                    CalculateScore(eventMatch, matchBet, bet.Distance);
                }
            }

            return filteredBets;
        }

        public async Task<RoundBets> GetUserBets(string userId)
        {
            var eventId = _configurationService.EventId;

            RoundInfoDetails roundInfo = await _snookerFeedService.GetCurrentRound();
            if (roundInfo == null || roundInfo.Started)
            {
                return null;
            }

            var result = await _betsRepository.GetUserBets(userId, roundInfo.Round);
            if (result != null)
            {
                return result;
            }

            var matches = await _snookerFeedService.GetRoundMatches(roundInfo.Round);
            var players = await _snookerFeedService.GetEventPlayers();

            var roundBets = new RoundBets
            {
                UserId = userId,
                EventId = eventId,
                RoundId = roundInfo.Round,
                Distance = roundInfo.Distance,
                MatchBets = new List<Bet>()
            };

            foreach (var match in matches)
            {
                var bet = new Bet
                {
                    MatchId = match.MatchId,
                    Player1Id = match.Player1Id,
                    Player1Name = players.Single(p => p.Id == match.Player1Id).ToString(),
                    Score1 = null,
                    Player2Id = match.Player2Id,
                    Player2Name = players.Single(p => p.Id == match.Player2Id).ToString(),
                    Score2 = null
                };

                roundBets.MatchBets.Add(bet);
            }

            return roundBets;
        }

        public async Task<bool> SubmitBets(string userId, RoundBets bets)
        {
            var eventId = _configurationService.EventId;

            var canSubmitBets = await CanSubmitBets();
            if (!canSubmitBets)
            {
                return false;
            }

            bets.UserId = userId;
            bets.EventId = eventId;
            bets.UpdatedAt = DateTime.Now;
            await _betsRepository.SubmitBets(bets);
            return true;
        }

        private async Task<bool> CanSubmitBets()
        {
            var currentRound =  await _snookerFeedService.GetCurrentRound();
            var startRound = _configurationService.StartRound;
            return currentRound != null && !currentRound.Started && currentRound.Round >= startRound;
        }

        private void CalculateScore(Match eventMatch, Bet matchBet, int matchDistance)
        {
            if (eventMatch.WinnerId == matchBet.WinnerId)
            {
                var error = matchBet.WinnerId == matchBet.Player1Id
                    ? Math.Abs(eventMatch.Score2 - matchBet.Score2.Value)
                    : Math.Abs(eventMatch.Score1 - matchBet.Score1.Value);

                matchBet.ScoreValue = Math.Max(1, matchDistance / Math.Pow(2, error));
                matchBet.Error = error;
            }
            else
            {
                matchBet.ScoreValue = 0.0;
                matchBet.Error = null;
            }
        }
    }
}