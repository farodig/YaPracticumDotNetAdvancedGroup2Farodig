using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

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
    }
}
