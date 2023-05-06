using System.Collections.Generic;
using System.Threading.Tasks;
using BetSnooker.Models.API;

namespace BetSnooker.Services.Interfaces
{
    public interface ISnookerCacheService
    {
        Task<IEnumerable<Event>> GetEvents(int season);
        Task<Event> GetEvent(int eventId);
        Task<Match> GetMatch(int eventId, int roundId, int matchNumber);
        Task<Player> GetPlayer(int playerId);
        Task<IEnumerable<Match>> GetEventMatches(int eventId);
        Task<IEnumerable<Match>> GetOngoingMatches();
        Task<IEnumerable<Player>> GetEventPlayers(int eventId);
        Task<IEnumerable<RoundInfo>> GetEventRounds(int eventId);
    }
}