using LearningWebApi.Repositories;
using LearningWebApi.Services.EventService;

namespace LearningTest.Factories
{
    /// <summary>
    /// Вспомогательный класс для инициализации репозитория
    /// </summary>
    public static class EventServiceFactory
    {
        /// <summary>
        /// Создание сервиса событий с подготовленными данными для тестов
        /// использование moq пока что супер избыточно (написание тестов ради тестов|кода ради кода)
        /// </summary>
        public static IEventService CreateEventService(params IEnumerable<Event> events)
        {
            var eventRepository = new EventRepository() as IEventRepository;
            foreach (var item in events)
            {
                eventRepository.Add(item.Id, item);
            }
            return new EventService(eventRepository);
        }
    }
}
