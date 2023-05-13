using System;
using System.Collections.Generic;
using System.Linq;
using BetSnooker.Models;
using BetSnooker.Services.Interfaces;

namespace BetSnooker.Services
{
    public class ScoreCalculation : IScoreCalculation
    {
        public IEnumerable<EventBets> CalculateAllScores(IEnumerable<RoundBets> eventBets, IEnumerable<MatchDetails> eventMatches, RoundInfoDetails currentRound)
        {
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
                    UserScore = new UserScore
                    {
                        MatchesFinished = matchesCount,
                        EventScore = eventScore,
                        CorrectWinners = correctWinners,
                        ExactScores = exactScores,
                        CorrectWinnersAccuracy = matchesCount != 0 ? (double)correctWinners / matchesCount : 0.0,
                        ExactScoresAccuracy = matchesCount != 0 ? (double)exactScores / matchesCount : 0.0,
                        AverageError = matchesWithErrorsCount != 0 ? (double)aggregatedErrors / matchesWithErrorsCount : null
                    }
                });
            }

            // mark the winner/s
            MarkWinners(currentRound, allUsersEventBets);

            return allUsersEventBets;
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

        private void MarkWinners(RoundInfoDetails currentRound, IEnumerable<EventBets> allUsersEventBets)
        {
            if (currentRound.IsFinalRound && currentRound.Finished)
            {
                var maxScore = allUsersEventBets.Max(b => b.UserScore.EventScore);
                foreach (var bet in allUsersEventBets.Where(bet => AreEqual(bet.UserScore.EventScore, maxScore)))
                {
                    bet.UserScore.IsWinner = true;
                }
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
