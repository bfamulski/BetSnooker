using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Helpers;
using BetSnooker.Models;
using BetSnooker.Repositories;

namespace BetSnooker.Services
{
    public interface IAuthenticationService
    {
        Task<bool> Register(User user);
        Task<User> Login(Credentials credentials);
        Task<IEnumerable<User>> GetUsers();
    }
    
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;

        public AuthenticationService(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }

        public async Task<bool> Register(User user)
        {
            user.Password = PasswordHelper.HashPassword(user.Password);

            try
            {
                return await _authenticationRepository.AddUser(user);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<User> Login(Credentials credentials)
        {
            var user = await Task.Run(() =>
                _authenticationRepository.GetUser(credentials.Username, PasswordHelper.HashPassword(credentials.Password)));
            return user?.WithoutPassword();
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _authenticationRepository.GetUsers();
            return users.Select(user => user.WithoutPassword());
        }
    }
}