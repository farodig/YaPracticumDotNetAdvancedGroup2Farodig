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
        public static IEventService CreateEventService(IEventRepository repository)
        {
            return new EventService(repository);
        }

        public static IEventService CreateEventService(params IEnumerable<Event> events)
        {
            var eventRepository = CreateEventRepository(events);
            return CreateEventService(eventRepository);
        }

        public static IBookingService CreateBookingService()
        {
            var bookingRepository = CreateBookingRepository();
            var eventService = CreateEventService();
            return new BookingService(eventService, bookingRepository);
        }

        public static IBookingService CreateBookingService(IEventService eventService)
        {
            var bookingRepository = CreateBookingRepository();
            return new BookingService(eventService, bookingRepository);
        }

        public static IBookingService CreateBookingService(IBookingRepository bookingRepository)
        {
            var eventService = CreateEventService();
            return new BookingService(eventService, bookingRepository);
        }

        public static IBookingService CreateBookingService(IBookingRepository bookingRepository, IEventService eventService)
        {
            return new BookingService(eventService, bookingRepository);
        }

        internal static async Task<BackgroundService> CreateBookingProcessor(IBookingService bookingService, IEventRepository eventRepository)
        {
            var processor = new BookingProcessor(bookingService, eventRepository);
            await processor.StartAsync(CancellationToken.None);

            return processor;
        }
    }
}
