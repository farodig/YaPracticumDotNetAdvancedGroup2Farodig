using LearningWebApi.Entities;

namespace LearningTest.Helpers
{
    internal static class EntityFactory
    {
        public static Event CreateEvent() => new()
        {
            Id = Guid.NewGuid(),
        };

        public static Event CreateEvent(Action<Event>? configure = null)
        {
            var eventData = CreateEvent();
            configure?.Invoke(eventData);
            return eventData;
        }

        public static Booking CreateBooking() => new()
        {
            Id = Guid.NewGuid(),
        };
    }
}
