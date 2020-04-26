using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSnookerApi.Models;

namespace TestSnookerApi.Repositories
{
    public interface IPlayersRepository
    {
        IEnumerable<Player> GetEventPlayers(int eventId);

        Player GetPlayer(int playerId);

        Task SetPlayers(Player[] players);
    }

    public class PlayersRepository : IPlayersRepository
    {
        private readonly InMemoryDbContext _context;

        public PlayersRepository(InMemoryDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Player> GetEventPlayers(int eventId)
        {
            return _context.Players;
        }

        public Player GetPlayer(int playerId)
        {
            return _context.Players.SingleOrDefault(p => p.Id == playerId);
        }

        public async Task SetPlayers(Player[] players)
        {
            _context.Players.RemoveRange(_context.Players);
            await _context.Players.AddRangeAsync(players);
            await _context.SaveChangesAsync();
        }
    }
}