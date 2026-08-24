using BrokerService.Application;
using Moq;
using SharedContracts.Abstractions;

namespace BookingService.UnitTest.Helpers
{
    internal static class MockBuilder
    {
        public static IPublishService GetPublishServiceMock()
        {
            var publishServiceMock = new Mock<IPublishService>();

            publishServiceMock
                .Setup(x => x.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return publishServiceMock.Object;
        }
    }
}
