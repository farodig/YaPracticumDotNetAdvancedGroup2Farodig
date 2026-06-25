using LearningWebApi.Entities;
using LearningWebApi.Repositories;

namespace LearningTest.Factories
{
    internal static class RepositoryFactory
    {
        public static IEventRepository CreateEventRepository(params IEnumerable<Event> items)
        {
            var repository = new EventRepository() as IEventRepository;

            foreach (var item in items)
            {
                repository.Add(item.Id, item);
            }

            return repository;
        }

        public static IBookingRepository CreateBookingRepository() => new BookingRepository()
        {

        };
    }
}
