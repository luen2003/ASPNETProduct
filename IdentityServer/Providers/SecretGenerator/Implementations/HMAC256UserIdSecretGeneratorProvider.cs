using IdentityServer.Guards;
using IdentityServer.Providers.SecretGenerator;
using System;
using System.Security.Cryptography;
using System.Text;
using Guard = IdentityServer.Guards.Guard;

namespace IdentityServer.Providers.SecretGenerator.Implementations
{
    public class HMAC256UserIdSecretGeneratorProvider : ISecretGeneratorProvider
    {
        public string GenerateSecret(string from)
        {
            Guard.ThrowIfArgumentNull(from, nameof(from)); // we need this to be set up

            var stringBytes = Encoding.ASCII.GetBytes(from);
            using var hmac = new HMACSHA256(stringBytes);
            var hashBytes = hmac.ComputeHash(stringBytes);
            var hashString = Convert.ToBase64String(hashBytes);
            return hashString;
        }
    }
}
