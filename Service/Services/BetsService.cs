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
        private const int UnknownPlayerId = 376;

        private readonly IBetsRepository _betsRepository;
        private readonly ISnookerFeedService _snookerFeedService;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ILogger _logger;

        public BetsService(IBetsRepository betsRepository, ISnookerFeedService snookerFeedService, ISettingsProvider settingsProvider, ILogger<BetsService> logger)
        {
            _betsRepository = betsRepository;
            _snookerFeedService = snookerFeedService;
            _settingsProvider = settingsProvider;
            _logger = logger;
        }

        // TODO: refactor this method
        public async Task<IEnumerable<EventBets>> GetEventBets()
        {
            var eventId = _settingsProvider.EventId;

            var eventRounds = _snookerFeedService.GetEventRounds().ToList();
            if (!eventRounds.Any())
            {
                _logger.LogWarning("No event rounds available");
                return null;
            }

            var currentRound = _snookerFeedService.GetCurrentRound(eventRounds);
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

            var eventMatches = _snookerFeedService.GetEventMatches().ToList();

            var allUsersEventBets = new List<EventBets>();
            var eventBetsGroupedByUser = eventBets.GroupBy(b => b.UserId);
            foreach (var betsGrouped in eventBetsGroupedByUser)
            {
                int matchesCount = 0;
                double eventScore = 0.0;
                int correctWinners = 0;
                int exactScores = 0;
                int aggregatedErrors = 0;
                int matchesWithErrorsCount = 0;

                foreach (var userRoundBets in betsGrouped)
                {
                    double roundScore = 0.0;
                    foreach (var matchBet in userRoundBets.MatchBets)
                    {
                        // mark bet as placed
                        if (matchBet.Score1.HasValue && matchBet.Score2.HasValue)
                        {
                            matchBet.BetPlaced = true;
                        }

                        var eventMatch = eventMatches.SingleOrDefault(m => m.MatchId == matchBet.MatchId);
                        if (eventMatch == null)
                        {
                            continue;
                        }

                        // do not return match bet if match has not yet started
                        if (eventMatch.ActualStartDate != null && eventMatch.ActualStartDate.Value.ToLocalTime() > DateTime.Now)
                        {
                            matchBet.Score1 = null;
                            matchBet.Score2 = null;
                            continue;
                        }

                        // skip walk-overs
                        if (eventMatch.WinnerId == 0 || eventMatch.Walkover1 || eventMatch.Walkover2)
                        {
                            continue;
                        }

                        // calculate stats
                        matchesCount++;
                        CalculateScore(eventMatch, matchBet, userRoundBets.Distance);

                        if (matchBet.ScoreValue.HasValue)
                        {
                            roundScore += matchBet.ScoreValue.Value;
                        }

                        if (eventMatch.Score1 == matchBet.Score1 && eventMatch.Score2 == matchBet.Score2)
                        {
                            exactScores++;
                        }

                        if (eventMatch.WinnerId == matchBet.WinnerId)
                        {
                            correctWinners++;
                        }

                        if (matchBet.Error.HasValue)
                        {
                            aggregatedErrors += matchBet.Error.Value;
                            matchesWithErrorsCount++;
                        }
                    }

                    userRoundBets.RoundScore = roundScore;
                    eventScore += userRoundBets.RoundScore.Value;
                }

                allUsersEventBets.Add(new EventBets
                                      {
                                          UserId = betsGrouped.Key,
                                          RoundBets = betsGrouped,
                                          MatchesFinished = matchesCount,
                                          EventScore = eventScore,
                                          CorrectWinners = correctWinners,
                                          ExactScores = exactScores,
                                          CorrectWinnersAccuracy = matchesCount != 0 ? (double)correctWinners / matchesCount : 0.0,
                                          ExactScoresAccuracy = matchesCount != 0 ? (double)exactScores / matchesCount : 0.0,
                                          AverageError = matchesWithErrorsCount != 0 ? (double)aggregatedErrors / matchesWithErrorsCount : (double?)null
                                      });
            }

            // mark the winner/s
            MarkWinners();

            return allUsersEventBets;

            void MarkWinners()
            {
                if (currentRound.IsFinalRound && currentRound.Finished)
                {
                    var maxScore = allUsersEventBets.Max(b => b.EventScore);
                    foreach (var bet in allUsersEventBets.Where(bet => AreEqual(bet.EventScore, maxScore)))
                    {
                        bet.IsWinner = true;
                    }
                }
            }
        }

        public async Task<IEnumerable<RoundBets>> GetUserBets(string userId)
        {
            var eventId = _settingsProvider.EventId;

            RoundInfoDetails currentRound = _snookerFeedService.GetCurrentRound(null);
            if (currentRound == null)
            {
                return null;
            }

            var players = _snookerFeedService.GetEventPlayers().ToList();

            var currentRoundBets = await GetUserBetsForRound(userId, eventId, currentRound, players);

            var userBets = new List<RoundBets> { currentRoundBets };

            if (currentRound.Started)
            {
                RoundInfoDetails nextRound = _snookerFeedService.GetNextRound(currentRound);
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

        // this logic is handled in the controller
        public async Task<SubmitResult> SubmitBetsV2(string userId, IEnumerable<RoundBets> bets)
        {
            foreach (var roundBets in bets)
            {
                var canSubmitBets = CanSubmitBets(roundBets);
                if (!canSubmitBets)
                {
                    return SubmitResult.InvalidRound;
                }

                var betsInvalid = ValidateBets(roundBets);
                if (betsInvalid)
                {
                    return SubmitResult.ValidationError;
                }

                roundBets.UserId = userId;
                roundBets.EventId = _settingsProvider.EventId;
                roundBets.UpdatedAt = DateTime.UtcNow;

                try
                {
                    await _betsRepository.SubmitBets(roundBets);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return SubmitResult.InternalServerError;
                }
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

        private void CalculateScore(MatchDetails eventMatch, Bet matchBet, int matchDistance)
        {
            if (eventMatch.WinnerId == 0 || eventMatch.Walkover1 || eventMatch.Walkover2)
            {
                matchBet.ScoreValue = null;
                return;
            }

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

        private async Task<RoundBets> GetUserBetsForRound(string userId, int eventId, RoundInfoDetails round, List<Player> players)
        {
            var matches = _snookerFeedService.GetRoundMatches(round.Round).ToList();

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
            return match.Player1Id != UnknownPlayerId &&
                   match.Player2Id != UnknownPlayerId &&
                   !match.Walkover1 &&
                   !match.Walkover2 &&
                   match.ActualStartDate.HasValue &&
                   match.ActualStartDate.Value.ToLocalTime() >= DateTime.Now;
        }

        private bool AreEqual(double? val1, double? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return false;
            }

            return Math.Abs(val1.Value - val2.Value) <= double.Epsilon;
        }
    }
}