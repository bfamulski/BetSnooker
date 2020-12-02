using System.Collections.Generic;
using System.Threading.Tasks;
using BetSnooker.Models;

namespace BetSnooker.Repositories.Interfaces
{
    public interface IBetsRepository
    {
        IEnumerable<RoundBets> GetAllBets(int eventId, int[] rounds);
        Task<RoundBets> GetUserBets(string userId, int eventId, int roundId);
        Task SubmitBets(RoundBets bets);
    }
}