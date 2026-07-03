using LearningWebApi.DataAccess;
using LearningWebApi.Entities;
using LearningWebApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LearningTest.Factories
{
    internal static class RepositoryFactory
    {
        public static async Task<IEventRepository> CreateEventRepositoryAsync(params IEnumerable<Event> items)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var repository = new EventRepository(new AppDbContext(options));

            foreach (var item in items)
            {
                await repository.CreateAsync(item);
            }

            return repository;
        }

        public static IBookingRepository CreateBookingRepository() => new BookingRepository()
        {

        };
    }
}
