using System.Security.Cryptography;
using System.Text;

namespace Application.Components
{
    public class SHA256PasswordHasher : IPasswordHasher
    {
        public string GenerateHash(string password)
        {
            return GenerateHashInternal(password);
        }

        public bool Verify(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            var actualHash = GenerateHashInternal(password);

            return actualHash.Equals(hash, StringComparison.Ordinal);
        }

        private string GenerateHashInternal(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}
