using System.Collections.Generic;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetSnooker.Repositories
{
    public class UserSubscriptionsRepository : IUserSubscriptionsRepository
    {
        private readonly DatabaseContext _context;

        public UserSubscriptionsRepository(DatabaseContext context)
        {
            _context = context;
        }

        public IEnumerable<UserSubscription> Get()
        {
            return _context.UserSubscriptions.AsNoTracking();
        }

        public async Task<bool> Add(UserSubscription subscription)
        {
            var subscriptionEntity = await _context.UserSubscriptions.FirstOrDefaultAsync(sub =>
                sub.Endpoint == subscription.Endpoint && sub.P256DH == subscription.P256DH && sub.Auth == subscription.Auth);
            if (subscriptionEntity == null)
            {
                await _context.UserSubscriptions.AddAsync(subscription);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}