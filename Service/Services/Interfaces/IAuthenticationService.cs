using System.Collections.Generic;
using System.Threading.Tasks;
using BetSnooker.Models;

namespace BetSnooker.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User> Register(User user);
        Task<User> Login(Credentials credentials);
        Task<IEnumerable<User>> GetUsers();
    }
}