using Domain.Entities;

namespace Application.Repositories
{
    /// <summary>
    /// Репозиторий пользователей
    /// </summary>
    public interface IPersonRepository
    {
        /// <summary>
        /// Получить пользователя по логину
        /// </summary>
        Task<Person?> GetByLoginAsync(string login, CancellationToken cts);

        /// <summary>
        /// Создать пользователя
        /// </summary>
        Task CreateAsync(Person person, CancellationToken cts);
    }
}
