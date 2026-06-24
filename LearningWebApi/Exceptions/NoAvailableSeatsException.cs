namespace LearningWebApi.Exceptions
{
    /// <summary>
    /// Недостаточное количество мест
    /// </summary>
    public class NoAvailableSeatsException : Exception
    {
        /// <summary>
        /// Вызывать исключение с текстом по умолчанию
        /// </summary>
        public NoAvailableSeatsException() : base("No available seats for this event")
        {
        }
    }
}
