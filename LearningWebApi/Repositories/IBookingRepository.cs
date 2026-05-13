using LearningWebApi.Services.BookingService;
using System.Diagnostics.CodeAnalysis;

namespace LearningWebApi.Repositories
{
    /// <summary>
    /// Репозиторий бронирований
    /// </summary>
    public interface IBookingRepository : IDictionary<Guid, Booking>
    {
    }
}
