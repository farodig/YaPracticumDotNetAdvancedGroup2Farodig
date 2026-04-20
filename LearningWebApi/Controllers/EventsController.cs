using LearningWebApi.Models;
using LearningWebApi.Models.Requests;
using LearningWebApi.Models.Responses;
using LearningWebApi.Services.EventService;
using Microsoft.AspNetCore.Mvc;

namespace LearningWebApi.Controllers
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
        public IActionResult GetEvents(
            [FromQuery] string? title = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return Ok(new PaginatedResult
            {
                Items = _eventService
                .GetEvents()
                .FilterByTitle(title)
                .FilterByFrom(from)
                .FilterByTo(to)
                .Pagination(page, pageSize)
                .Select(EventFactory.ToEventRespose)
                .ToList(),
                PageNumber = page,
                TotalCount = _eventService.Count,
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
        public IActionResult GetEvent(Guid id)
        {
            if (_eventService.GetEvent(id) is not Event item)
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
        public IActionResult CreateEvent([FromBody] CreateEventRequest data)
        {
            var created = _eventService.CreateEvent(
                data.Title,
                data.StartAt!.Value,
                data.EndAt!.Value,
                data.Description)
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
        public IActionResult UpdateEvent([FromRoute] Guid id, [FromBody] UpdateEventRequest data)
        {
            var toUpdate = data.CreateEvent(id);

            if (!_eventService.TryUpdateEvent(toUpdate))
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
        public IActionResult DeleteEvent(Guid id)
        {
            if (!_eventService.TryDeleteEvent(id))
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
