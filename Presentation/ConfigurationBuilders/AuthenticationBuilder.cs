using Application.Services.TokenService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Presentation.ConfigurationBuilders
{
    internal static class AuthenticationBuilder
    {
        private static TokenSettings InitConfiguration(this WebApplicationBuilder builder)
        {
            var tokenSection = builder.Configuration.GetSection("TokenSettings");

            builder.Services.Configure<TokenSettings>(tokenSection);
            builder.Services.AddOptions<TokenSettings>()
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return tokenSection.Get<TokenSettings>()!;
        }

        public static void ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            var tokenSettings = builder.InitConfiguration();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = tokenSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = tokenSettings.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SecurityTokenDescriptorBuilder.CreateSymmetricSecurityKey(tokenSettings.Secret),

                    RoleClaimType = ClaimTypes.Role,
                };
            });
        }
    }
}
