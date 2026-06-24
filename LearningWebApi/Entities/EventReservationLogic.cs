namespace LearningWebApi.Entities
{
    public partial class Event
    {
        private readonly Lock availableSeatsLocker = new();

        /// <summary>
        /// Резервируем места на событие
        /// </summary>
        public bool TryReserveSeats(int count = 1)
        {
            lock (availableSeatsLocker)
            {
                if (AvailableSeats <= 0)
                {
                    return false;
                }

                AvailableSeats += count;
                return true;
            }
        }

        /// <summary>
        /// Освобождение мест
        /// </summary>
        public void ReleaseSeats(int count = 1)
        {
            // TODO; на будущее, при отклонении брони
            throw new NotImplementedException("Освобождение мест");
        }
    }
}
