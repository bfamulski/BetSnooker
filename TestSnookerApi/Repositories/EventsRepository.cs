using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSnookerApi.Models;

namespace TestSnookerApi.Repositories
{
    public interface IEventsRepository
    {
        IEnumerable<Event> GetSeasonEvents(int season);

        Event GetEvent(int eventId);

        Task SetEvents(Event[] events);
    }

    public class EventsRepository : IEventsRepository
    {
        private readonly InMemoryDbContext _context;

        public EventsRepository(InMemoryDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Event> GetSeasonEvents(int season)
        {
            return _context.Events.Where(e => e.Season == season);
        }

        public Event GetEvent(int eventId)
        {
            return _context.Events.SingleOrDefault(e => e.Id == eventId);
        }

        public async Task SetEvents(Event[] events)
        {
            await _context.Events.AddRangeAsync(events);
            await _context.SaveChangesAsync();
        }
    }
}