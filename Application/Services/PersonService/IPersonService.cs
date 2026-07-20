using Application.Models.Responses;
using Domain.Entities;

namespace Application.Services.PersonService
{
    /// <summary>
    /// Сервис пользователей
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        Task<PersonResponse> LoginAsync(string login, string password, CancellationToken cts);

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        Task<PersonResponse> RegisterAsync(string login, string password, PersonRole role, CancellationToken cts);
    }
}
