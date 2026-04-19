using System.Security.Cryptography;
using System.Text;

namespace kurswork_back.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool Verify(string password, string hash);
    }
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool Verify(string password, string hash)
        {
            var computedHash = HashPassword(password);
            return computedHash == hash;
        }
    }
}
