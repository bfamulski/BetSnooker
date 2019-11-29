using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using Microsoft.EntityFrameworkCore;

namespace BetSnooker.Repositories
{
    public interface IAuthenticationRepository
    {
        Task<bool> AddUser(User user);
        User GetUser(string username, string password);
        Task<IEnumerable<User>> GetUsers();
    }

    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly InMemoryDbContext _context;

        public AuthenticationRepository(InMemoryDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddUser(User user)
        {
            if (_context.Users.FirstOrDefault(u => u.Username == user.Username) != null)
            {
                return false;
            }

            await _context.Users.AddAsync(user);
            var result = await _context.SaveChangesAsync();
            return result == 1;
        }

        public User GetUser(string username, string password)
        {
            return _context.Users.SingleOrDefault(x => x.Username == username && x.Password == password);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
    }
}