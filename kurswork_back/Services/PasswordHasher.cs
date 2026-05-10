using Microsoft.AspNetCore.Identity;

namespace kurswork_back.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool Verify(string password, string hash);
    }

    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<string> _hasher = new();

        public string HashPassword(string password)
        {
            return _hasher.HashPassword(null!, password);
        }

        public bool Verify(string password, string hash)
        {
            var result = _hasher.VerifyHashedPassword(null!, hash, password);
            return result != PasswordVerificationResult.Failed;
        }
    }
}