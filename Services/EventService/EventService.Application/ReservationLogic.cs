using EventService.Domain.Entities;

namespace EventService.Application
{
    internal static class ReservationLogic
    {
        /// <summary>
        /// Резервируем места на событие
        /// </summary>
        internal static bool TryReserveSeats(this Event item, int count = 1)
        {
            // Меняем состояние
            item.AvailableSeats -= count;

            // Проверяем валидность
            if (item.AvailableSeats < 0)
            {
                // При необходимости восстанавливаем
                item.AvailableSeats += count;
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Освобождение мест
        /// </summary>
        internal static void ReleaseSeats(this Event item, int count = 1)
        {
            // Меняем состояние
            item.AvailableSeats += count;

            // Проверяем валидность
            if (item.AvailableSeats > item.TotalSeats)
            {
                // При необходимости восстанавливаем
                item.AvailableSeats = item.TotalSeats;
            }
        }
    }
}
