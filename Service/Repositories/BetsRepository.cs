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
        private readonly InMemoryDbContext _context;

        public BetsRepository(InMemoryDbContext context)
        {
            _context = context;
        }

        public IEnumerable<RoundBets> GetAllBets(int[] rounds)
        {
            return _context.RoundBets.Include("MatchBets").AsNoTracking().Where(bet => rounds.Contains(bet.RoundId));
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
                foreach (var matchBet in betsEntity.MatchBets)
                {
                    matchBet.Score1 = bets.MatchBets.Single(m => m.MatchId == matchBet.MatchId).Score1;
                    matchBet.Score2 = bets.MatchBets.Single(m => m.MatchId == matchBet.MatchId).Score2;
                }

                _context.RoundBets.Update(betsEntity);
            }

            await _context.SaveChangesAsync();
        }
    }
}