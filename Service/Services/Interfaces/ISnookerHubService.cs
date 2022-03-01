using System.Collections.Generic;
using BetSnooker.Models.API;

namespace BetSnooker.Services.Interfaces
{
    public interface ISnookerHubService : IHubService
    {
        Event GetEvent();
        IEnumerable<Match> GetEventMatches();
        IEnumerable<Player> GetEventPlayers();
        IEnumerable<RoundInfo> GetEventRounds();
    }
}