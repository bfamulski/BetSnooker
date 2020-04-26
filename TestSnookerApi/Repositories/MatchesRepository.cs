using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSnookerApi.Models;

namespace TestSnookerApi.Repositories
{
    public interface IMatchesRepository
    {
        IEnumerable<Match> GetEventMatches(int eventId);

        Match GetMatch(int eventId, int roundId, int matchNumber);

        IEnumerable<Match> GetOngoingMatches();

        Task SetMatches(Match[] matches);

        Task UpdateMatches(Match[] matches);
    }

    public class MatchesRepository : IMatchesRepository
    {
        private readonly InMemoryDbContext _context;

        public MatchesRepository(InMemoryDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Match> GetEventMatches(int eventId)
        {
            return _context.Matches.Where(m => m.EventId == eventId);
        }

        public Match GetMatch(int eventId, int roundId, int matchNumber)
        {
            return _context.Matches.SingleOrDefault(m => m.EventId == eventId && m.Round == roundId && m.Number == matchNumber);
        }

        public IEnumerable<Match> GetOngoingMatches()
        {
            throw new System.NotImplementedException();
        }

        public async Task SetMatches(Match[] matches)
        {
            _context.Matches.RemoveRange(_context.Matches);
            await _context.Matches.AddRangeAsync(matches);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMatches(Match[] matches)
        {
            _context.Matches.UpdateRange(matches);
            await _context.SaveChangesAsync();
        }
    }
}