using LearningWebApi.Entities;

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
            if (page <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page), $"The parameter must be positive and above zero");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"The parameter must be positive and above zero");
            }

            return data.OrderBy(c => c.StartAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
    }
}
