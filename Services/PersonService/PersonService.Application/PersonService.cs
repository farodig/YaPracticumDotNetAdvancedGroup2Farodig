using Microsoft.Extensions.Logging;
using PersonService.Application.Abstractions;
using PersonService.Application.Components;
using PersonService.Domain.Entities;
using PersonService.Domain.Exceptions;
using SharedContracts.Enums;
using TokenService;

namespace PersonService.Application
{
    public class PersonService(IPersonRepository repository, IPasswordHasher passwordHasher, ITokenService tokenService, ILogger<PersonService> logger) : IPersonService
    {
        private readonly ILogger<PersonService> _logger = logger;
        private readonly IPersonRepository _repository = repository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<string> LoginAsync(string login, string password, CancellationToken cts = default)
        {
            Person person = await _repository.GetByLoginAsync(login, cts) ?? throw new WrongLoginOrPasswordException();

            if (!_passwordHasher.Verify(password, person.PasswordHash)) throw new WrongLoginOrPasswordException();

            var token = _tokenService.CreateToken(person.Id, person.Role.ToString());

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Person #{Id} logged with role '{Role}'", person.Id, person.Role);
            }

            return token;
        }

        public async Task RegisterAsync(string login, string password, PersonRole role = PersonRole.User, CancellationToken cts = default)
        {
            var person = new Person
            {
                Id = Guid.NewGuid(),
                Login = login,
                Role = role,
                PasswordHash = _passwordHasher.GenerateHash(password),
            };

            await _repository.CreateAsync(person, cts);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Person #{Id} created with role '{Role}'", person.Id, person.Role);
            }
        }
    }
}
