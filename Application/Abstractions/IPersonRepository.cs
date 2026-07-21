using Domain.Entities;

namespace Application.Abstractions
{
    /// <summary>
    /// Репозиторий пользователей
    /// </summary>
    public interface IPersonRepository
    {
        /// <summary>
        /// Получить пользователя по логину
        /// </summary>
        Task<Person?> GetByLoginAsync(string login, CancellationToken cts = default);

        /// <summary>
        /// Создать пользователя
        /// </summary>
        Task CreateAsync(Person person, CancellationToken cts = default);
    }
}
