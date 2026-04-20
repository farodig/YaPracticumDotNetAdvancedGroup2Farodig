namespace LearningWebApi.Services.EventService
{
    /// <summary>
    /// Постраничный вывод событий
    /// </summary>
    public interface IEventPagination
    {
        /// <summary>
        /// Общее количество событий
        /// </summary>
        public int Count { get; }
    }
}
