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
        Task<string> LoginAsync(string login, string password, CancellationToken cts);

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        Task<string> RegisterAsync(string login, string password, PersonRole role, CancellationToken cts);
    }
}
