using LearningWebApi.Services.BookingService;
using System.Diagnostics.CodeAnalysis;

namespace LearningWebApi.Repositories
{
    /// <summary>
    /// Репозиторий бронирований
    /// </summary>
    public interface IBookingRepository : IDictionary<Guid, Booking>
    {
        /// <summary>
        /// Обновить бронь
        /// </summary>
        bool TryUpdate(Guid key, Booking newValue, Booking oldValue);

        /// <summary>
        /// Удалить событие
        /// </summary>
        bool TryRemove(Guid key, [MaybeNullWhen(false)] out Booking deleted);
    }
}
