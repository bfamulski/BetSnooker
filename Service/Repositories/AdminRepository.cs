using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models.API;

namespace BetSnooker.Repositories
{
    public interface IAdminRepository
    {
        Event GetCurrentEvent();
        Task<bool> SetCurrentEvent(Event @event);
        void SetStartRound(int roundId);
    }

    public class AdminRepository : IAdminRepository
    {
        private readonly InMemoryDbContext _context;

        public AdminRepository(InMemoryDbContext context)
        {
            _context = context;
        }

        public Event GetCurrentEvent()
        {
            // TODO: come up with better criteria
            //return _context.Events.LastOrDefault();

            // TODO: remove after tests
            return new Event { Id = 857, Name = "UK Championship", Season = 2019 };
        }

        public async Task<bool> SetCurrentEvent(Event @event)
        {
            await _context.Events.AddAsync(@event);
            var result = await _context.SaveChangesAsync();
            return result == 1;
        }

        public void SetStartRound(int roundId)
        {
            // TODO: set in in server state
        }
    }
}