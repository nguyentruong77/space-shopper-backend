using SpaceShopper.Application.Interfaces.Security;
using System.Security.Cryptography;
using System.Text;

namespace SpaceShopper.Infrastructure.Security
{
    public sealed class CacheKeyHashService : ICacheKeyHashService
    {
        public string Hash(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
