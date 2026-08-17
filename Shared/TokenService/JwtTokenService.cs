using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using PersonService.Domain.Entities;
using System.Security.Claims;
using TokenService.Builders;
using TokenService.Exceptions;

namespace TokenService
{
    public class JwtTokenService(IOptions<TokenSettings> tokenSettings) : ITokenService
    {
        private readonly TokenSettings _tokenSettings = tokenSettings.Value;

        public string CreateToken(Guid personId, string role)
        {
            var descriptor = SecurityTokenDescriptorBuilder
                .Create(_tokenSettings)
                .BuildClaims(personId, role)
                .BuildCredential(_tokenSettings);

            return new JsonWebTokenHandler().CreateToken(descriptor);
        }

        public Guid GetPersonId(ClaimsPrincipal user)
        {
            string claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedBookingOperationException();

            return Guid.Parse(claim);
        }

        public bool IsAdmin(ClaimsPrincipal user)
        {
            string claim = user.FindFirst(ClaimTypes.Role)?.Value
                ?? throw new UnauthorizedBookingOperationException();
            
            var personRole = Enum.Parse<PersonRole>(claim);

            return personRole is PersonRole.Admin;
        }
    }
}
