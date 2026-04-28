namespace LearningWebApi.Models.Responses
{
    /// <summary>
    /// Постраничный вывод событий
    /// </summary>
    public class PaginatedResult
    {
        /// <summary>
        /// Массив самих событий
        /// </summary>
        public required List<EventResponse> Items { get; set; }

        /// <summary>
        /// Номер текущей страницы
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Количество элементов на текущей странице
        /// </summary>
        public int PageCount => Items.Count;

        /// <summary>
        /// Общее количество событий
        /// </summary>
        public int TotalCount { get; set; }
    }
}
