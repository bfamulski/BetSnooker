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
            byte[] passwordWithSaltBytes = Encoding.UTF8.GetBytes(string.Concat(password, Salt));

            var hasher = new SHA256CryptoServiceProvider();
            byte[] hashedBytes = hasher.ComputeHash(passwordWithSaltBytes);
            hasher.Clear();
            return Convert.ToBase64String(hashedBytes);
        }
    }
}