using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace Fliq.Application.Common.Security
{
    public class PasswordHash
    {
        public static string Create(string password, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 80000,
                numBytesRequested: 256 / 8);
            return Convert.ToBase64String(valueBytes);
        }

        public static bool Validate(string password, string salt, string hash)
            => Create(password, salt) == hash;
    }
}
