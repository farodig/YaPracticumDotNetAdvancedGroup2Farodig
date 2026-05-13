using LearningWebApi.Services.BookingService;
using System.Collections.Concurrent;

namespace LearningWebApi.Repositories
{
    internal class BookingRepository : ConcurrentDictionary<Guid, Booking>, IBookingRepository
    {
    }
}
