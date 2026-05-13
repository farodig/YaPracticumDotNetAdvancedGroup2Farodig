namespace LearningWebApi.Entities
{
    /// <summary>
    /// Статус бронирования
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// Бронь создана, ожидаение обработки
        /// </summary>
        Pending,

        /// <summary>
        /// Бронь подтверждена
        /// </summary>
        Confirmed,

        /// <summary>
        /// Бронь отклонена
        /// </summary>
        Rejected, 
    }
}
