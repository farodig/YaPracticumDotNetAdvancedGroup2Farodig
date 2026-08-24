using SharedContracts.Enums;

namespace PersonService.Application.Abstractions
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
        Task RegisterAsync(string login, string password, PersonRole role, CancellationToken cts);
    }
}
