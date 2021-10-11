using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Configuration;
using BetSnooker.Models;
using BetSnooker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetSnooker.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly int _maxUsers;
        private readonly DatabaseContext _context;
        
        public AuthenticationRepository(DatabaseContext context, ISettingsProvider settings)
        {
            _context = context;
            _maxUsers = settings.MaxUsers;
        }

        public async Task<bool> AddUser(User user)
        {
            if (_context.Users.FirstOrDefault(u => u.Username == user.Username) != null)
            {
                return false;
            }

            if (_context.Users.Count() >= _maxUsers)
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