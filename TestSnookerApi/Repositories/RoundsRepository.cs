using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSnookerApi.Models;

namespace TestSnookerApi.Repositories
{
    public interface IRoundsRepository
    {
        IEnumerable<RoundInfo> GetEventRounds(int eventId);

        Task SetRounds(RoundInfo[] rounds);
    }

    public class RoundsRepository : IRoundsRepository
    {
        private readonly InMemoryDbContext _context;

        public RoundsRepository(InMemoryDbContext context)
        {
            _context = context;
        }

        public IEnumerable<RoundInfo> GetEventRounds(int eventId)
        {
            return _context.Rounds.Where(r => r.EventId == eventId);
        }

        public async Task SetRounds(RoundInfo[] rounds)
        {
            await _context.Rounds.AddRangeAsync(rounds);
            await _context.SaveChangesAsync();
        }
    }
}