namespace SharedContracts.Events.BookingEvents
{
    public enum CancelReasonType : int
    {
        /// <summary>
        /// Бронь отменена администратором
        /// </summary>
        CancelByAdmin,

        /// <summary>
        /// Бронь отменена самим пользователем
        /// </summary>
        CancelByPerson,
    }
}
