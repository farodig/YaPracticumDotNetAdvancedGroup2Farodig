using Microsoft.Extensions.DependencyInjection;
using PersonService.Application.Components;
using TokenService;

namespace PersonService.Application
{
    public static class Injection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<ITokenService, TokenService.TokenService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddSingleton<IPasswordHasher, SHA256PasswordHasher>();
        }
    }
}
