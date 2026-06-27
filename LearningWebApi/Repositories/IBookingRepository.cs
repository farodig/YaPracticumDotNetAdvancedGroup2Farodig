using LearningWebApi.Entities;

namespace LearningWebApi.Repositories
{
    /// <summary>
    /// Репозиторий бронирований
    /// </summary>
    public interface IBookingRepository : IDictionary<Guid, Booking>
    {
        /// <summary>
        /// Получить бронь по идентификатору
        /// </summary>
        Booking? Get(Guid id);

        /// <summary>
        /// Создать или обновить бронь
        /// </summary>
        void CreateOrUpdate(Booking item);

        /// <summary>
        /// Удалить бронирование
        /// </summary>
        new void Remove(Guid id);
    }
}
