using System.Collections.Generic;
using System.Threading.Tasks;
using BetSnooker.Models;

namespace BetSnooker.Services.Interfaces
{
    public interface IBetsService
    {
        Task<IEnumerable<RoundBets>> GetAllBets();
        Task<RoundBets> GetUserBets(string userId);
        Task<bool> SubmitBets(string userId, RoundBets bets);
    }
}