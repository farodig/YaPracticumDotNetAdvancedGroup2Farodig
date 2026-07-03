using LearningWebApi.Entities;
using LearningWebApi.Repositories;
using LearningWebApi.Services.BookingService;
using LearningWebApi.Services.EventService;
using Microsoft.Extensions.DependencyInjection;
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

        public static async Task<IEventService> CreateEventService(params IEnumerable<Event> events)
        {
            var eventRepository = await CreateEventRepositoryAsync(events);
            return CreateEventService(eventRepository);
        }

        public static async Task<IBookingService> CreateBookingService()
        {
            var bookingRepository = CreateBookingRepository();
            var eventService = await CreateEventService();
            return new BookingService(eventService, bookingRepository);
        }

        public static IBookingService CreateBookingService(IEventService eventService)
        {
            var bookingRepository = CreateBookingRepository();
            return new BookingService(eventService, bookingRepository);
        }

        public static async Task<IBookingService> CreateBookingService(IBookingRepository bookingRepository)
        {
            var eventService = await CreateEventService();
            return new BookingService(eventService, bookingRepository);
        }

        internal static BookingProcessor CreateBookingProcessor(IBookingService bookingService, IEventService eventService)
        {
            var services = new ServiceCollection();
            services.AddSingleton(bookingService);
            services.AddSingleton(eventService);
            var serviceProvider = services.BuildServiceProvider();
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            return new BookingProcessor(scopeFactory);
        }
    }
}
