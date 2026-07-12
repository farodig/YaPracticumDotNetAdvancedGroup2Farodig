using Domain.Entities;

namespace LearningWebApi.Entities
{
    internal static class EventReservationLogic
    {
        /// <summary>
        /// Резервируем места на событие
        /// </summary>
        internal static bool TryReserveSeats(this Event item, int count = 1)
        {
            if (item.AvailableSeats <= 0)
            {
                return false;
            }

            item.AvailableSeats -= count;
            return true;
        }

        /// <summary>
        /// Освобождение мест
        /// </summary>
        internal static void ReleaseSeats(this Event item, int count = 1)
        {
            item.AvailableSeats += count;

            if (item.AvailableSeats > item.TotalSeats)
            {
                item.AvailableSeats = item.TotalSeats;
            }
        }
    }
}
