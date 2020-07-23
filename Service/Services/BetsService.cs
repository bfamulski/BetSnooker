using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Configuration;
using BetSnooker.Models;
using BetSnooker.Repositories.Interfaces;
using BetSnooker.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BetSnooker.Services
{
    public class BetsService : IBetsService
    {
        private readonly IBetsRepository _betsRepository;
        private readonly ISnookerFeedService _snookerFeedService;
        private readonly ISettings _settings;
        private readonly ILogger _logger;

        public BetsService(IBetsRepository betsRepository, ISnookerFeedService snookerFeedService, ISettings settings, ILogger<BetsService> logger)
        {
            _betsRepository = betsRepository;
            _snookerFeedService = snookerFeedService;
            _settings = settings;
            _logger = logger;
        }

        // TODO: refactor this method
        public async Task<IEnumerable<EventBets>> GetEventBets()
        {
            var currentRound = _snookerFeedService.GetCurrentRound();
            if (currentRound == null)
            {
                return null;
            }

            var eventRounds = _snookerFeedService.GetEventRounds();
            var filteredRounds = currentRound.Started
                ? eventRounds.Where(r => r.Round <= currentRound.Round).ToList()
                : eventRounds.Where(r => r.Round < currentRound.Round).ToList();
            if (!filteredRounds.Any())
            {
                return null;
            }

            var eventBets = await Task.Run(() => _betsRepository.GetAllBets(filteredRounds.Select(r => r.Round).ToArray()));
            if (eventBets == null || !eventBets.Any())
            {
                return null;
            }

            var eventMatches = _snookerFeedService.GetEventMatches();

            var allUsersEventBets = new List<EventBets>();
            var eventBetsGroupedByUser = eventBets.GroupBy(b => b.UserId);
            foreach (var betsGrouped in eventBetsGroupedByUser)
            {
                int matchesCount = 0;
                double eventScore = 0.0;
                int correctWinners = 0;
                int exactScores = 0;
                foreach (var userRoundBets in betsGrouped)
                {
                    double roundScore = 0.0;
                    foreach (var matchBet in userRoundBets.MatchBets)
                    {
                        var eventMatch = eventMatches.Single(m => m.MatchId == matchBet.MatchId);
                        if (eventMatch.WinnerId == 0 || eventMatch.Walkover1 || eventMatch.Walkover2)
                        {
                            continue;
                        }

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
                                          ExactScoresAccuracy = matchesCount != 0 ? (double)exactScores / matchesCount : 0.0
                                      });
            }

            // mark the winner/s
            if (currentRound.IsFinalRound && currentRound.Finished)
            {
                var maxScore = allUsersEventBets.Max(b => b.EventScore);
                foreach (var bet in allUsersEventBets.Where(bet => AreEqual(bet.EventScore, maxScore)))
                {
                    bet.IsWinner = true;
                }
            }

            return allUsersEventBets;
        }

        public async Task<RoundBets> GetUserBets(string userId)
        {
            var eventId = _settings.EventId;

            RoundInfoDetails roundInfo = _snookerFeedService.GetCurrentRound();
            if (roundInfo == null || roundInfo.Started)
            {
                return null;
            }

            var result = await _betsRepository.GetUserBets(userId, roundInfo.Round);
            if (result != null)
            {
                return result;
            }

            var matches = _snookerFeedService.GetRoundMatches(roundInfo.Round).ToList();
            var players = _snookerFeedService.GetEventPlayers().ToList();

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

        public async Task<SubmitResult> SubmitBets(string userId, RoundBets bets)
        {
            var canSubmitBets = CanSubmitBets();
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
            bets.EventId = _settings.EventId;
            bets.UpdatedAt = DateTime.Now; // TODO: UtcNow?

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

        private bool CanSubmitBets()
        {
            var currentRound = _snookerFeedService.GetCurrentRound();
            var startRound = _settings.StartRound;
            return currentRound != null && !currentRound.Started && currentRound.Round >= startRound;
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