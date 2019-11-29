using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using Microsoft.EntityFrameworkCore;

namespace BetSnooker.Repositories
{
    public interface IBetsRepository
    {
        IEnumerable<RoundBets> GetAllBets(int roundId);
        RoundBets GetBets(string userId, int roundId);
        Task SubmitBets(string userId, RoundBets bets);
        bool BetsSent(string userId, int eventId, int roundId);
        bool DeleteBets(int eventId, int roundId);
    }

    public class BetsRepository : IBetsRepository
    {
        private readonly InMemoryDbContext _context;

        public BetsRepository(InMemoryDbContext context)
        {
            _context = context;
        }

        public IEnumerable<RoundBets> GetAllBets(int roundId)
        {
            return _context.RoundBets.Include("MatchBets").Where(bets => bets.RoundId == roundId);
        }

        public RoundBets GetBets(string userId, int roundId)
        {
            return _context.RoundBets.Include("MatchBets").OrderByDescending(bet => bet.UpdatedAt)
                .FirstOrDefault(bet => bet.UserId == userId && bet.RoundId == roundId);
        }

        public async Task SubmitBets(string userId, RoundBets bets)
        {
            bets.UserId = userId;
            bets.UpdatedAt = DateTime.Now;

            var betsEntity = _context.RoundBets.Include("MatchBets").FirstOrDefault(bet =>
                bet.UserId == userId && bet.EventId == bets.EventId && bet.RoundId == bets.RoundId);
            if (betsEntity == null)
            {
                await _context.RoundBets.AddAsync(bets);
            }
            else
            {
                betsEntity.UpdatedAt = bets.UpdatedAt;
                betsEntity.MatchBets = bets.MatchBets;
                _context.RoundBets.Update(betsEntity);
            }

            await _context.SaveChangesAsync();
        }

        public bool BetsSent(string userId, int eventId, int roundId)
        {
            var result = _context.RoundBets.FirstOrDefault(bet =>
                bet.UserId == userId && bet.EventId == eventId && bet.RoundId == roundId && bet.MatchBets.Any());
            return result != null;
        }

        public bool DeleteBets(int eventId, int roundId)
        {
            var bets = _context.RoundBets.Where(bet => bet.EventId == eventId && bet.RoundId == roundId).ToList();
            _context.RoundBets.RemoveRange(bets);
            _context.SaveChanges();
            return true;
        }
    }
}