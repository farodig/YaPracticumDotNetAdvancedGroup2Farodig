using Application.Abstractions;
using Application.Components;
using Domain.Entities;
using Domain.Exceptions;
using NLog;

namespace Application.Services.PersonService
{
    public class PersonService(IPersonRepository repository, IPasswordHasher passwordHasher, ITokenService tokenService) : IPersonService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPersonRepository _repository = repository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<string> LoginAsync(string login, string password, CancellationToken cts = default)
        {
            Person person = await _repository.GetByLoginAsync(login, cts) ?? throw new WrongLoginOrPasswordException();

            if (!_passwordHasher.Verify(password, person.PasswordHash)) throw new WrongLoginOrPasswordException();

            var token = _tokenService.CreateToken(person);

            _logger.Info($"Person #{person.Id} logged with role '{person.Role}', token '{token}'");

            return token;
        }

        public async Task<string> RegisterAsync(string login, string password, PersonRole role = PersonRole.User, CancellationToken cts = default)
        {
            var person = new Person
            {
                Id = Guid.NewGuid(),
                Login = login,
                Role = role,
                PasswordHash = _passwordHasher.GenerateHash(password),
            };

            await _repository.CreateAsync(person, cts);

            var token = _tokenService.CreateToken(person);

            _logger.Info($"Person #{person.Id} created with role '{person.Role}', token '{token}'");

            return token;
        }
    }
}
