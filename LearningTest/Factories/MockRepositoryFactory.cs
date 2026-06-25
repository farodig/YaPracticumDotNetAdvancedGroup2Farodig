using LearningWebApi.Entities;
using LearningWebApi.Repositories;
using Moq;

namespace LearningTest.Factories
{
    internal static class MockRepositoryFactory
    {
        public static IEventRepository MockEventRepository(params Event[] items)
        {
            var mockRepository = new Mock<IEventRepository>();
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                mockRepository
                    .Setup(repo => repo.TryGetValue(item.Id, out item))
                    .Returns(true);

                mockRepository
                    .Setup(repo => repo.ContainsKey(item.Id))
                    .Returns(true);
            }

            return mockRepository.Object;
        }

        public static IBookingRepository MockBookingRepository(params Booking[] items)
        {
            var mockRepository = new Mock<IBookingRepository>();
            for (int i = 0; i < items.Length; i++)
            {
                var @event = items[i];
                mockRepository
                    .Setup(repo => repo.TryGetValue(@event.Id, out @event))
                    .Returns(true);

                //mockRepository
                //    .Setup(repo => repo.ContainsKey(@event.Id))
                //    .Returns(true);
            }

            return mockRepository.Object;
        }
    }
}
