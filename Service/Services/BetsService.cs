using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Configuration;
using BetSnooker.Models;
using BetSnooker.Models.API;
using BetSnooker.Repositories.Interfaces;
using BetSnooker.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BetSnooker.Services
{
    public class BetsService : IBetsService
    {
        private readonly IBetsRepository _betsRepository;
        private readonly ISnookerFeedService _snookerFeedService;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IScoreCalculation _scoreCalculation;
        private readonly ILogger _logger;

        public BetsService(
            IBetsRepository betsRepository,
            ISnookerFeedService snookerFeedService,
            ISettingsProvider settingsProvider,
            IScoreCalculation scoreCalculation,
            ILogger<BetsService> logger)
        {
            _betsRepository = betsRepository;
            _snookerFeedService = snookerFeedService;
            _settingsProvider = settingsProvider;
            _scoreCalculation = scoreCalculation;
            _logger = logger;
        }

        public async Task<IEnumerable<EventBets>> GetEventBets()
        {
            var eventId = _settingsProvider.EventId;

            var eventRounds = await _snookerFeedService.GetEventRounds();
            if (!eventRounds.Any())
            {
                _logger.LogWarning("No event rounds available");
                return null;
            }

            var currentRound = await _snookerFeedService.GetCurrentRound(eventRounds);
            if (currentRound == null)
            {
                _logger.LogWarning("Current round is not available");
                return null;
            }

            var eventBets = await Task.Run(() => _betsRepository.GetAllBets(eventId, eventRounds.Select(r => r.Round).ToArray()));
            if (eventBets == null || !eventBets.Any())
            {
                _logger.LogInformation("No event bets available");
                return null;
            }

            var eventMatches = await _snookerFeedService.GetEventMatches();

            var allUsersEventBets = _scoreCalculation.CalculateAllScores(eventBets, eventMatches, currentRound);
            return allUsersEventBets;
        }

        public async Task<IEnumerable<RoundBets>> GetUserBets(string userId)
        {
            var eventId = _settingsProvider.EventId;

            RoundInfoDetails currentRound = await _snookerFeedService.GetCurrentRound(null);
            if (currentRound == null)
            {
                return null;
            }

            var players = (await _snookerFeedService.GetEventPlayers()).ToList();

            var currentRoundBets = await GetUserBetsForRound(userId, eventId, currentRound, players);

            var userBets = new List<RoundBets> { currentRoundBets };

            if (currentRound.Started)
            {
                RoundInfoDetails nextRound = await _snookerFeedService.GetNextRound(currentRound);
                if (nextRound != null)
                {
                    var nextRoundBets = await GetUserBetsForRound(userId, eventId, nextRound, players);
                    userBets.Add(nextRoundBets);
                }
            }

            return userBets.AsReadOnly();
        }

        public async Task<SubmitResult> SubmitBets(string userId, RoundBets bets)
        {
            var canSubmitBets = CanSubmitBets(bets);
            if (!canSubmitBets)
            {
                return SubmitResult.InvalidRound;
            }

            var betsInvalid = ValidateBets(bets);
            if (betsInvalid)
            {
                return SubmitResult.ValidationError;
            }

            bets.UserId = userId;
            bets.EventId = _settingsProvider.EventId;
            bets.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _betsRepository.SubmitBets(bets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return SubmitResult.InternalServerError;
            }
            
            return SubmitResult.Success;
        }

        private bool CanSubmitBets(RoundBets bets)
        {
            var startRound = _settingsProvider.StartRound;
            return bets.RoundId >= startRound && bets.MatchBets.All(matchBet => matchBet.Active);
        }

        private bool ValidateBets(RoundBets bets)
        {
            var maxScore = bets.Distance;
            return bets.MatchBets.Any(bet => (bet.Score1 == null && bet.Score2 != null)
                                             || (bet.Score1 != null && bet.Score2 == null)
                                             || (bet.Score1 != null && bet.Score2 != null
                                                 && (bet.Score1 == bet.Score2 
                                                     || bet.Score1 > maxScore || bet.Score2 > maxScore
                                                     || bet.Score1 < 0 || bet.Score2 < 0
                                                     || (bet.Score1 < maxScore && bet.Score2 < maxScore))));
        }

        private async Task<RoundBets> GetUserBetsForRound(string userId, int eventId, RoundInfoDetails round, List<Player> players)
        {
            var matches = (await _snookerFeedService.GetRoundMatches(round.Round)).ToList();

            var userBets = new RoundBets();

            var result = await _betsRepository.GetUserBets(userId, eventId, round.Round);
            if (result != null)
            {
                userBets.UserId = result.UserId;
                userBets.EventId = result.EventId;
                userBets.RoundId = result.RoundId;
                userBets.Distance = result.Distance;
                userBets.UpdatedAt = result.UpdatedAt;
            }
            else
            {
                userBets.UserId = userId;
                userBets.EventId = eventId;
                userBets.RoundId = round.Round;
                userBets.Distance = round.Distance;
            }

            userBets.MatchBets = new List<Bet>();
            foreach (var match in matches)
            {
                var bet = result?.MatchBets?.SingleOrDefault(m => m.MatchId == match.MatchId);
                if (bet != null)
                {
                    bet.Active = IsBetActive(match);
                    bet.MatchStartDate = match.ActualStartDate;
                    bet.Player1Name = players.Single(p => p.Id == match.Player1Id).ToString();
                    bet.Player2Name = players.Single(p => p.Id == match.Player2Id).ToString();
                }
                else
                {
                    bet = new Bet
                    {
                        Active = IsBetActive(match),
                        MatchId = match.MatchId,
                        Player1Id = match.Player1Id,
                        Player1Name = players.Single(p => p.Id == match.Player1Id).ToString(),
                        Score1 = null,
                        Player2Id = match.Player2Id,
                        Player2Name = players.Single(p => p.Id == match.Player2Id).ToString(),
                        Score2 = null,
                        MatchStartDate = match.ActualStartDate
                    };
                }

                userBets.MatchBets.Add(bet);
            }

            return userBets;
        }

        private bool IsBetActive(MatchDetails match)
        {
            return match.Player1Id != _settingsProvider.UnknownPlayerId &&
                   match.Player2Id != _settingsProvider.UnknownPlayerId &&
                   !match.Walkover1 &&
                   !match.Walkover2 &&
                   match.ActualStartDate.HasValue &&
                   match.ActualStartDate.Value.ToLocalTime() >= DateTime.Now;
        }
    }
}