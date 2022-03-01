using System.Collections.Generic;
using System.Threading.Tasks;
using BetSnooker.Models;

namespace BetSnooker.Repositories.Interfaces
{
    public interface IUserSubscriptionsRepository
    {
        IEnumerable<UserSubscription> Get();
        Task<bool> Add(UserSubscription subscription);
    }
}