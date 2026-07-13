using Application.Models.Requests;
using System.ComponentModel.DataAnnotations;

namespace Learning.UnitTests.EventServiceTests
{
    [Trait("Category", "Unit")]
    public class EventValidationTest
    {
        [Theory(DisplayName = "01. Создание события с некорректными данными")]
        [InlineData("", -1, 1, 10, nameof(CreateEventRequest.Title))]
        [InlineData("incorrectStart", -1, 1, 10, nameof(CreateEventRequest.StartAt))]
        [InlineData("incorrectBothDate", 1, -1, 10, nameof(CreateEventRequest.EndAt))]
        [InlineData("totalSeatsZero", -1, 1, 0, nameof(CreateEventRequest.TotalSeats))]
        [InlineData("totalSeatsNegative", -1, 1, -1, nameof(CreateEventRequest.TotalSeats))]
        public void CreateEventFailTest(string title, int diffStart, int diffEnd, int totalSeats, string memberName)
        {
            var request = new CreateEventRequest
            {
                Title = title,
                StartAt = DateTime.Now.AddHours(diffStart),
                EndAt = DateTime.Now.AddHours(diffEnd),
                TotalSeats = totalSeats,
            };
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains(memberName));
        }

        [Theory(DisplayName = "02. Обновление события с некорректными датами (EndAt раньше StartAt)")]
        [InlineData("", -1, 1, nameof(CreateEventRequest.Title))]
        [InlineData("incorrectStart", -1, 1, nameof(CreateEventRequest.StartAt))]
        [InlineData("incorrectBothDate", 1, -1, nameof(CreateEventRequest.EndAt))]
        public void UpdateEventFailTest(string title, int diffStart, int diffEnd, string memberName)
        {
            var request = new UpdateEventRequest
            {
                Title = title,
                StartAt = DateTime.Now.AddHours(diffStart),
                EndAt = DateTime.Now.AddHours(diffEnd),
            };
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains(memberName));
        }
    }
}
