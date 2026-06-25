using LearningWebApi.Entities;

namespace LearningWebApi.Repositories
{
    /// <summary>
    /// Репозиторий бронирований
    /// </summary>
    public interface IBookingRepository : IDictionary<Guid, Booking>
    {
        /// <summary>
        /// Сохранить данные в репозитории
        /// </summary>
        void SaveData();
    }
}
