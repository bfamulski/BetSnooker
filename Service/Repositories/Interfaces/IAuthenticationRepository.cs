using System.Collections.Generic;
using System.Threading.Tasks;
using BetSnooker.Models;

namespace BetSnooker.Repositories.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task<bool> AddUser(User user);
        User GetUser(string username, string password);
        Task<IEnumerable<User>> GetUsers();
    }
}