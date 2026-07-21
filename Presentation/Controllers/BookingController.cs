using Application.Abstractions;
using Application.Models.Responses;
using Application.Services.BookingService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// Конечная точка доступа сервиса событий
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class BookingController(IBookingService bookingService, ITokenService tokenService) : ControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;
        private readonly ITokenService _tokenService = tokenService;

        /// <summary>
        /// Забронировать
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <response code="202">Бронирование в обработке</response>
        /// <response code="404">Событие не найдено</response>
        /// <response code="409">Конфликт бронирования мест события</response>
        [HttpPost("/events/{id}/book")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status202Accepted, "application/json")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict, "application/json")]
        public async Task<ActionResult<BookingResponse>> CreateBooking(Guid id)
        {
            var personId = _tokenService.GetPersonId(User);
            var item = await _bookingService.CreateBookingAsync(id, personId, HttpContext.RequestAborted);

            return AcceptedAtAction(
                actionName: nameof(GetBooking),
                routeValues: new { id = item.Id },
                value: item);
        }

        /// <summary>
        /// Узнать статус бронирования
        /// </summary>
        /// <param name="id">Идентификатор брони</param>
        /// <response code="200">Информация о бронировании получена</response>
        /// <response code="404">Бронирование не найдено</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK, "application/json")]
        public async Task<ActionResult<BookingResponse>> GetBooking(Guid id)
        {
            var item = await _bookingService.GetBookingByIdAsync(id, HttpContext.RequestAborted);
            return Ok(item);
        }

        /// <summary>
        /// Удалить бронирование
        /// </summary>
        /// <param name="id">Идентификатор брони</param>
        /// <response code="200">Бронирование успешно удалено</response>
        /// <response code="403">Нет прав на удаление брони</response>
        /// <response code="404">Бронирование не найдено</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var personId = _tokenService.GetPersonId(User);
            var role = _tokenService.GetRole(User);

            await _bookingService.CancelBookingAsync(id, personId, role, HttpContext.RequestAborted);
            return Ok();
        }
    }
}
