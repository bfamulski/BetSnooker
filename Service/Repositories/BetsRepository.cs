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
            return _context.RoundBets.Include("MatchBets").Where(bet => rounds.Contains(bet.RoundId));
        }

        public async Task<RoundBets> GetUserBets(string userId, int roundId)
        {
            return await _context.RoundBets.Include("MatchBets").SingleOrDefaultAsync(bet => bet.UserId == userId && bet.RoundId == roundId);
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
                _context.RoundBets.Remove(betsEntity);
                await _context.RoundBets.AddAsync(bets);

                //betsEntity.UpdatedAt = bets.UpdatedAt;
                //betsEntity.MatchBets = bets.MatchBets;
                //_context.Entry(betsEntity).State = EntityState.Modified;
                ////_context.RoundBets.Update(betsEntity);
            }

            await _context.SaveChangesAsync();
        }
    }
}