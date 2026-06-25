using LearningWebApi.Entities;
using System.Collections.Concurrent;

namespace LearningWebApi.Repositories
{
    internal class BookingRepository : ConcurrentDictionary<Guid, Booking>, IBookingRepository
    {
        public void SaveData()
        {
            // заглушка для БД всё в памяти, сохранять не нужно!
        }
    }
}
