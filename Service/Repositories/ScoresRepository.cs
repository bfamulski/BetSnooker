using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using Microsoft.EntityFrameworkCore;

namespace BetSnooker.Repositories
{
    public interface IScoresRepository
    {
        IEnumerable<Score> GetAllScores();
        IEnumerable<Score> GetScores(string userId, int eventId, int? roundId, int? matchId);
        Task SaveScore(Score score);
    }

    public class ScoresRepository : IScoresRepository
    {
        private readonly InMemoryDbContext _context;

        public ScoresRepository(InMemoryDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Score> GetAllScores()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Score> GetScores(string userId, int eventId, int? roundId, int? matchId)
        {
            var scores = _context.Scores.Where(score => score.UserId == userId && score.EventId == eventId);
            if (roundId.HasValue)
            {
                scores = scores.AsNoTracking().Where(score => score.RoundId == roundId);
            }

            if (matchId.HasValue)
            {
                scores = scores.AsNoTracking().Where(score => score.MatchId == matchId);
            }

            return scores;
        }

        public async Task SaveScore(Score score)
        {
            await _context.Scores.AddAsync(score);
            await _context.SaveChangesAsync();
        }
    }
}