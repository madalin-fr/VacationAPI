using System.Security.Cryptography;
using System.Text;
using VacationAPI.Models;

namespace VacationAPI.Helpers
{
    public static class PasswordHelper
    {
        public static bool VerifyPasswordHashAndSalt(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA256(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i])
                {
                    return false;
                }
            }
            return true;
        }
        public static (byte[], byte[]) CreatePasswordHashAndSalt(string password)
        {
            // Generate a random salt value
            byte[] passwordSalt;
            using (var rng = RandomNumberGenerator.Create())
            {
                var saltBytes = new byte[64];
                rng.GetBytes(saltBytes);
                passwordSalt = saltBytes;
            }

            // Compute the hash of the password using the salt value
            byte[] passwordHash;
            using (var hmac = new HMACSHA256(passwordSalt))
            {
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            // Return the password hash and salt as a tuple
            return (passwordHash, passwordSalt);
        }
    }
}
