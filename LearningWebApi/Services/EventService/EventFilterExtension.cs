namespace LearningWebApi.Services.EventService
{
    /// <summary>
    /// Фильтрация событий
    /// </summary>
    public static class EventFilterExtension
    {
        /// <summary>
        /// Поиск по названию (регистронезависимый, частичное совпадение)
        /// </summary>
        public static IEnumerable<Event> FilterByTitle(this IEnumerable<Event> data, string? title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                data = data.Where(a => a.Title.Contains(title, StringComparison.InvariantCultureIgnoreCase));
            }

            return data;
        }

        /// <summary>
        /// События, которые начинаются не раньше указанной даты
        /// </summary>
        public static IEnumerable<Event> FilterByFrom(this IEnumerable<Event> data, DateTime? dateTime)
        {
            if (dateTime.HasValue && dateTime != DateTime.MinValue)
            {
                data = data.Where(a => dateTime <= a.StartAt);
            }

            return data;
        }

        /// <summary>
        /// События, которые заканчиваются не позже указанной даты
        /// </summary>
        public static IEnumerable<Event> FilterByTo(this IEnumerable<Event> data, DateTime? dateTime)
        {
            if (dateTime.HasValue && dateTime != DateTime.MinValue)
            {
                data = data.Where(a => a.EndAt <= dateTime);
            }

            return data;
        }
    }
}
