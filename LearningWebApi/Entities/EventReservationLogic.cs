namespace LearningWebApi.Entities
{
    public partial class Event
    {
        // TODO: есть проблема с availableSeatsLocker, очевидно что lock не распространяется на запись в базу данных
        // передавать репозиторий в объект Event не комильфо... так же как вытаскивать логику резервирования мест в сервис без availableSeatsLocker...
        // решить это можно кешированием, но могу предположить что это не задача для 4 спринта т. к. сейчас и так всё хранится в памяти...
        // Если по этому поводу будут замечания в ревью, то жду не только факта, но и возможного решения проблемы
        private readonly Lock availableSeatsLocker = new();

        /// <summary>
        /// Резервируем места на событие
        /// </summary>
        internal bool TryReserveSeats(int count = 1)
        {
            lock (availableSeatsLocker)
            {
                if (AvailableSeats <= 0)
                {
                    return false;
                }

                AvailableSeats -= count;
                return true;
            }
        }

        /// <summary>
        /// Освобождение мест
        /// </summary>
        internal void ReleaseSeats(int count = 1)
        {
            lock (availableSeatsLocker)
            {
                AvailableSeats += count;

                if (AvailableSeats > TotalSeats)
                {
                    AvailableSeats = TotalSeats;
                }
            }
        }
    }
}
