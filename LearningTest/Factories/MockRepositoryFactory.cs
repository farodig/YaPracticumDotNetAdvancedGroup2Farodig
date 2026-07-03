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
                    .Setup(repo => repo.GetAsync(item.Id, It.IsAny<CancellationToken?>()))
                    .ReturnsAsync((Event?)item);
            }

            return mockRepository.Object;
        }

        public static IBookingRepository MockBookingRepository(params Booking[] items)
        {
            var mockRepository = new Mock<IBookingRepository>();
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                mockRepository
                    .Setup(repo => repo.Get(item.Id))
                    .Returns(item);
            }

            return mockRepository.Object;
        }
    }
}
