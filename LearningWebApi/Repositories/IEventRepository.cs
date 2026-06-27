using LearningWebApi.Entities;
using System.Diagnostics.CodeAnalysis;

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
        Event? GetEvent(Guid eventId);

        /// <summary>
        /// Обновить событие
        /// </summary>
        void CreateOrUpdate(Event item);

        /// <summary>
        /// Удалить событие
        /// </summary>
        bool TryRemove(Guid key, [MaybeNullWhen(false)] out Event deleted);
    }
}
