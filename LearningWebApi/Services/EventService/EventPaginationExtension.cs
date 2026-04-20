namespace LearningWebApi.Services.EventService
{
    /// <summary>
    /// Расширение для постраничного вывода событий
    /// </summary>
    public static class EventPaginationExtension
    {
        /// <summary>
        /// Постраничный вывод событий
        /// </summary>
        public static IEnumerable<Event> Pagination(this IEnumerable<Event> data, int page, int pageSize)
        {
            return data.OrderBy(c => c.StartAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
    }
}
