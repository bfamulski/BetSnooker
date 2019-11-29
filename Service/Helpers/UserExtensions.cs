using System.Collections.Generic;
using System.Linq;
using BetSnooker.Models;

namespace BetSnooker.Helpers
{
    public static class UserExtensions
    {
        public static IEnumerable<User> WithoutPasswords(this IEnumerable<User> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static User WithoutPassword(this User user)
        {
            if (user == null)
            {
                return null;
            }

            var secureUser = new User
            {
                Id = user.Id,
                Username = user.Username,
                Password = null,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return secureUser;
        }
    }
}