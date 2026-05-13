using LearningWebApi.Entities;
using System.Collections.Concurrent;

namespace LearningWebApi.Repositories
{
    internal class BookingRepository : ConcurrentDictionary<Guid, Booking>, IBookingRepository
    {
    }
}
