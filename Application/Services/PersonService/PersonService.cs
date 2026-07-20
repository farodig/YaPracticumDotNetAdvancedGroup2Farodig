using Application.Components;
using Application.Models.Builders;
using Application.Models.Responses;
using Application.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using NLog;

namespace Application.Services.PersonService
{
    public class PersonService(IPersonRepository repository, IPasswordHasher passwordHasher) : IPersonService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPersonRepository _repository = repository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<PersonResponse> LoginAsync(string login, string password, CancellationToken cts = default)
        {
            Person person = await _repository.GetByLoginAsync(login, cts) ?? throw new WrongLoginOrPasswordException();

            if (!_passwordHasher.Verify(password, person.PasswordHash)) throw new WrongLoginOrPasswordException();

            _logger.Info($"Person #{person.Id} logged with role '{person.Role}'");

            return person.BuildPersonResponse();
        }

        public async Task<PersonResponse> RegisterAsync(string login, string password, PersonRole role = PersonRole.User, CancellationToken cts = default)
        {
            var person = new Person
            {
                Id = Guid.NewGuid(),
                Login = login,
                Role = role,
                PasswordHash = _passwordHasher.GenerateHash(password),
            };

            await _repository.CreateAsync(person, cts);

            _logger.Info($"Person #{person.Id} created with role '{person.Role}'");

            return person.BuildPersonResponse();
        }
    }
}
