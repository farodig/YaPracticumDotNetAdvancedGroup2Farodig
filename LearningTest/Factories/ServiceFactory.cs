using LearningWebApi.Entities;
using LearningWebApi.Repositories;
using LearningWebApi.Services.BookingService;
using LearningWebApi.Services.EventService;
using Microsoft.Extensions.Hosting;
using static LearningTest.Factories.RepositoryFactory;

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
            var eventRepository = CreateEventRepository(events);
            return new EventService(eventRepository);
        }

        public static IBookingService CreateBookingService()
        {
            var bookingRepository = CreateBookingRepository();
            var eventRepository = CreateEventRepository();
            return new BookingService(eventRepository, bookingRepository);
        }

        public static IBookingService CreateBookingService(IEventRepository eventRepository)
        {
            var bookingRepository = CreateBookingRepository();
            return new BookingService(eventRepository, bookingRepository);
        }

        public static IBookingService CreateBookingService(IBookingRepository bookingRepository)
        {
            var eventRepository = CreateEventRepository();
            return new BookingService(eventRepository, bookingRepository);
        }

        public static IBookingService CreateBookingService(IBookingRepository bookingRepository, IEventRepository eventRepository)
        {
            return new BookingService(eventRepository, bookingRepository);
        }

        internal static async Task<BackgroundService> CreateBookingProcessor(IBookingRepository bookingRepository, IEventRepository eventRepository)
        {
            var processor = new BookingProcessor(bookingRepository, eventRepository);
            await processor.StartAsync(CancellationToken.None);

            return processor;
        }
    }
}
