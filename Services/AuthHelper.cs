using System.Security.Cryptography;
using System.Text;

namespace InitialSetupMVC.Services
{
    public static class AuthHelper
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var hashed = HashPassword(enteredPassword);
            return string.Equals(hashed, storedHash, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
