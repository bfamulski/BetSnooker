using System.Collections.Generic;
using System.Threading.Tasks;
using BetSnooker.Models;

namespace BetSnooker.Repositories.Interfaces
{
    public interface IBetsRepository
    {
        IEnumerable<RoundBets> GetAllBets(int[] rounds);
        Task<RoundBets> GetUserBets(string userId, int roundId);
        Task SubmitBets(RoundBets bets);
    }
}