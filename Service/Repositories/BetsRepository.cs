using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetSnooker.Repositories
{
    public class BetsRepository : IBetsRepository
    {
        private readonly DatabaseContext _context;

        public BetsRepository(DatabaseContext context)
        {
            _context = context;
        }

        public IEnumerable<RoundBets> GetAllBets(int[] rounds)
        {
            return rounds == null || rounds.Length == 0
                ? _context.RoundBets.Include("MatchBets").AsNoTracking()
                : _context.RoundBets.Include("MatchBets").AsNoTracking().Where(bet => rounds.Contains(bet.RoundId));
        }

        public async Task<RoundBets> GetUserBets(string userId, int roundId)
        {
            return await _context.RoundBets.Include("MatchBets").AsNoTracking().SingleOrDefaultAsync(bet => bet.UserId == userId && bet.RoundId == roundId);
        }

        public async Task SubmitBets(RoundBets bets)
        {
            var betsEntity = _context.RoundBets.Include("MatchBets").FirstOrDefault(bet =>
                bet.UserId == bets.UserId && bet.EventId == bets.EventId && bet.RoundId == bets.RoundId);
            if (betsEntity == null)
            {
                await _context.RoundBets.AddAsync(bets);
            }
            else
            {
                betsEntity.UpdatedAt = bets.UpdatedAt;

                foreach (var matchBet in bets.MatchBets)
                {
                    var bet = betsEntity.MatchBets.SingleOrDefault(m => m.MatchId == matchBet.MatchId);
                    if (bet != null)
                    {
                        bet.Score1 = matchBet.Score1;
                        bet.Score2 = matchBet.Score2;
                    }
                    else
                    {
                        betsEntity.MatchBets.Add(matchBet);
                    }
                }

                _context.RoundBets.Update(betsEntity);
            }

            await _context.SaveChangesAsync();
        }
    }
}