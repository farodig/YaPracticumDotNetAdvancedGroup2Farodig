using Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Application.Services.TokenService
{
    internal static class SecurityTokenDescriptorBuilder
    {
        public static SecurityTokenDescriptor Create(TokenSettings settings) => new()
        {
            Issuer = settings.Issuer,
            Audience = settings.Audience,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(settings.ExpirationMin),
            IssuedAt = DateTime.UtcNow,
        };

        public static SecurityTokenDescriptor BuildClaims(this SecurityTokenDescriptor descriptor, Person person)
        {
            descriptor.Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, person.Id.ToString()),
                new Claim(ClaimTypes.Role, person.Role.ToString()),
            ]);
            return descriptor;
        }

        public static SecurityTokenDescriptor BuildCredential(this SecurityTokenDescriptor descriptor, TokenSettings settings)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
            descriptor.SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            return descriptor;
        }
    }
}
