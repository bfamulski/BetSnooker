using System.Collections.Generic;
using System.Threading.Tasks;
using BetSnooker.Models;

namespace BetSnooker.Services.Interfaces
{
    public enum SubmitResult
    {
        Success = 0,
        ValidationError = 1,
        InvalidRound = 2,
        InternalServerError = 3
    }

    public interface IBetsService
    {
        Task<IEnumerable<EventBets>> GetEventBets();
        Task<IEnumerable<RoundBets>> GetUserBets(string userId);
        Task<SubmitResult> SubmitBets(string userId, RoundBets bets);
    }
}