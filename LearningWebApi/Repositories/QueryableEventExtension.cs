using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningWebApi.Repositories
{
    /// <summary>
    /// Фильтрация событий Расширение для постраничного вывода событий
    /// </summary>
    internal static class QueryableEventExtension
    {
        /// <summary>
        /// Поиск по названию (регистронезависимый, частичное совпадение)
        /// </summary>
        public static IQueryable<Event> FilterByTitle(this IQueryable<Event> data, string? title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                data = data.Where(a => EF.Functions.ILike(a.Title, $"%{title}%"));
            }

            return data;
        }

        /// <summary>
        /// События, которые начинаются не раньше указанной даты
        /// </summary>
        public static IQueryable<Event> FilterByFrom(this IQueryable<Event> data, DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                data = data.Where(a => dateTime <= a.StartAt);
            }

            return data;
        }

        /// <summary>
        /// События, которые заканчиваются не позже указанной даты
        /// </summary>
        public static IQueryable<Event> FilterByTo(this IQueryable<Event> data, DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                data = data.Where(a => a.EndAt <= dateTime.Value);
            }

            return data;
        }

        /// <summary>
        /// Постраничный вывод событий
        /// </summary>
        public static IQueryable<Event> Pagination(this IQueryable<Event> data, int page, int pageSize)
        {
            if (page <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page), $"The parameter must be positive and above zero");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"The parameter must be positive and above zero");
            }

            var skipCount = (page - 1) * pageSize;
            return data.OrderBy(c => c.StartAt)
                .Skip(skipCount)
                .Take(pageSize);
        }
    }
}
