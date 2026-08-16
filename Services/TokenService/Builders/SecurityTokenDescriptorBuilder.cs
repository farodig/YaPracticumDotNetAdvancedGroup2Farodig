using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace TokenService.Builders
{
    public static class SecurityTokenDescriptorBuilder
    {
        public static SecurityTokenDescriptor Create(TokenSettings settings) => new()
        {
            Issuer = settings.Issuer,
            Audience = settings.Audience,
            NotBefore = DateTime.Now,
            Expires = DateTime.Now.AddMinutes(settings.ExpirationMin),
            IssuedAt = DateTime.Now,
        };

        public static SecurityTokenDescriptor BuildClaims(this SecurityTokenDescriptor descriptor, Guid personId, string role)
        {
            descriptor.Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, personId.ToString()),
                new Claim(ClaimTypes.Role, role),
            ]);
            return descriptor;
        }

        public static SecurityTokenDescriptor BuildCredential(this SecurityTokenDescriptor descriptor, TokenSettings settings)
        {
            var key = CreateSymmetricSecurityKey(settings.Secret);
            descriptor.SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            return descriptor;
        }

        public static SymmetricSecurityKey CreateSymmetricSecurityKey(string secret) => new(Encoding.UTF8.GetBytes(secret));
    }
}
