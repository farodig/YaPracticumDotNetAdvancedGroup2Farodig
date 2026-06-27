using LearningWebApi.Entities;

namespace LearningWebApi.Repositories
{
    /// <summary>
    /// Репозиторий событий
    /// </summary>
    public interface IEventRepository : IDictionary<Guid, Event>
    {
        /// <summary>
        /// Получить событие по идентификатору
        /// </summary>
        Event? Get(Guid id);

        /// <summary>
        /// Создать или обновить событие
        /// </summary>
        void CreateOrUpdate(Event item);

        /// <summary>
        /// Удалить событие
        /// </summary>
        new void Remove(Guid id);
    }
}
