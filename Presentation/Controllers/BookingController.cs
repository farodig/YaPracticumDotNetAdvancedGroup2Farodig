using Application.Models.Factories;
using Application.Models.Responses;
using Application.Services.BookingService;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// Конечная точка доступа сервиса событий
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class BookingController(IBookingService bookingService) : ControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;

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
            if (await _bookingService.CreateBookingAsync(id, HttpContext.RequestAborted) is not Booking item)
            {
                return NotFound();
            }

            return AcceptedAtAction(
                actionName: nameof(GetBooking),
                routeValues: new { id = item.Id },
                value: item.ToBookingResponse());
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
            if (await _bookingService.GetBookingByIdAsync(id, HttpContext.RequestAborted) is not Booking item)
            {
                return NotFound();
            }

            return Ok(item.ToBookingResponse());
        }
    }
}
