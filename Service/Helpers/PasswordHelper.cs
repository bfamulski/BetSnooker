using System;
using System.Security.Cryptography;
using System.Text;

namespace BetSnooker.Helpers
{
    public static class PasswordHelper
    {
        private const string Salt = "P@ssw0rdS@1lt";

        public static string HashPassword(string password)
        {
            using (var hasher = SHA256.Create())
            {
                byte[] passwordWithSaltBytes = Encoding.UTF8.GetBytes(string.Concat(password, Salt));
                byte[] hashedBytes = hasher.ComputeHash(passwordWithSaltBytes);                
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}