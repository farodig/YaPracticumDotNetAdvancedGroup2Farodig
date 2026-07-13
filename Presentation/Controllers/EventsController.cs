using Application.Models.Factories;
using Application.Models.Requests;
using Application.Models.Responses;
using Application.Services.EventService;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// Конечная точка доступа сервиса событий
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class EventsController(IEventService eventService) : ControllerBase
    {
        private readonly IEventService _eventService = eventService;

        /// <summary>
        /// Получить список всех событий
        /// </summary>
        /// <param name="title">Поиск событий по названию (опциональный, регистронезависимый, частичное совпадение)</param>
        /// <param name="from">Поиск событий которые начинаются не раньше указанной даты (опциональный)</param>
        /// <param name="to">Поиск событий которые заканчиваются не позже указанной даты (опциональный)</param>
        /// <param name="page">Страница, которую необходимо вернуть (опциональный, по умолчанию 1)</param>
        /// <param name="pageSize">Количество элементов на странице (опциональный, по умолчанию 10)</param>
        /// <response code="200">Список событий успешно возвращён</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> GetEvents(
            [FromQuery] string? title = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var filteredEvents = await _eventService.GetEventsAsync(page, pageSize, title, from, to, HttpContext.RequestAborted);
            
            return Ok(new PaginatedResult
            {
                Items = [.. filteredEvents.Select(EventFactory.ToEventRespose)],
                PageNumber = page,
                TotalCount = filteredEvents.Count(),
            });
        }

        /// <summary>
        /// Получить событие по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <response code="200">Событие успешно получено</response>
        /// <response code="404">Событие не найдено</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            if (await _eventService.GetEventAsync(id, HttpContext.RequestAborted) is not Event item)
            {
                return NotFound();
            }

            return Ok(item.ToEventRespose());
        }

        /// <summary>
        /// Создать событие
        /// </summary>
        /// <param name="data">Тело запроса</param>
        /// <response code="201">Событие успешно создано</response>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created, "application/json")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest data)
        {
            var created = (await _eventService.CreateEventAsync(
                data.Title,
                data.StartAt!.Value,
                data.EndAt!.Value,
                data.TotalSeats,
                data.Description,
                HttpContext.RequestAborted))
                .ToEventRespose();

            return CreatedAtAction(
                actionName: nameof(GetEvent),
                routeValues: new { id = created.Id },
                value: created);
        }

        /// <summary>
        /// Обновить событие целиком
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="data">Тело запроса</param>
        /// <response code="200">Событие успешно обновлено</response>
        /// <response code="404">Событие не найдено</response>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateEvent([FromRoute] Guid id, [FromBody] UpdateEventRequest data)
        {
            var toUpdate = data.CreateEvent(id);

            if (!await _eventService.TryUpdateEventAsync(toUpdate, HttpContext.RequestAborted))
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// Удалить событие
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <response code="200">Событие успешно удалено</response>
        /// <response code="404">Событие не найдено</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            if (!await _eventService.TryDeleteEventAsync(id, HttpContext.RequestAborted))
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
