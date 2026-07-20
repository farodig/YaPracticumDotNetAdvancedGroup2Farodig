using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Application.Services.TokenService
{
    public class TokenService(IOptions<TokenSettings> tokenSettings) : ITokenService
    {
        private readonly TokenSettings _tokenSettings = tokenSettings.Value;

        public string CreateToken(Person person)
        {
            var descriptor = SecurityTokenDescriptorBuilder
                .Create(_tokenSettings)
                .BuildClaims(person)
                .BuildCredential(_tokenSettings);

            return new JsonWebTokenHandler().CreateToken(descriptor);
        }

        public Guid GetPersonId(ClaimsPrincipal user)
        {
            string claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedBookingOperationException();

            return Guid.Parse(claim);
        }

        public PersonRole GetRole(ClaimsPrincipal user)
        {
            string claim = user.FindFirst(ClaimTypes.Role)?.Value
                ?? throw new UnauthorizedBookingOperationException();

            return Enum.Parse<PersonRole>(claim);
        }
    }
}
