using LearningWebApi.Entities;
using LearningWebApi.Repositories;
using LearningWebApi.Services.BookingService;
using LearningWebApi.Services.EventService;

namespace LearningTest.Factories
{
    /// <summary>
    /// Вспомогательный класс для инициализации репозитория
    /// </summary>
    public static class ServiceFactory
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

        public static IBookingService CreateBookingServiceWithEventRepository(IEventRepository? eventRepository = null)
        {
            var _bookingRepository = new BookingRepository() as IBookingRepository;
            var _eventRepository = eventRepository ?? new EventRepository() as IEventRepository;
            return new BookingService(_eventRepository, _bookingRepository);
        }

        public static IBookingService CreateBookingServiceWithBookingRepository(IBookingRepository? bookingRepository = null)
        {
            var _bookingRepository = bookingRepository ?? new BookingRepository() as IBookingRepository;
            var _eventRepository = new EventRepository() as IEventRepository;
            return new BookingService(_eventRepository, _bookingRepository);
        }

        public static IBookingService CreateBookingService(IBookingRepository? bookingRepository = null, IEventRepository? eventRepository = null)
        {
            var _bookingRepository = bookingRepository ?? new BookingRepository() as IBookingRepository;
            var _eventRepository = eventRepository ?? new EventRepository() as IEventRepository;
            return new BookingService(_eventRepository, _bookingRepository);
        }
    }
}
