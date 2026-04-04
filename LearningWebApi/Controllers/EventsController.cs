using LearningWebApi.Requests;
using LearningWebApi.Services.EventService;
using Microsoft.AspNetCore.Mvc;

namespace LearningWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController(IEventService eventService) : ControllerBase
    {
        private readonly IEventService _eventService = eventService;

        /// <summary>
        /// Получить список всех событий
        /// </summary>
        /// <response code="200">Список событий успешно возвращён</response> 
        [HttpGet]
        public IActionResult GetEvents()
        {
            return Ok(_eventService.GetEvents());
        }

        /// <summary>
        /// Получить событие по Id
        /// </summary>
        /// <param name="id">Идентификатор события, Guid</param>
        /// <response code="200">Событие успешно получено</response>
        /// <response code="404">Событие не найдено</response>
        [HttpGet("{id}")]
        public IActionResult GetEvent(Guid id)
        {
            if (_eventService.GetEvent(id) is not Event item)
            {
                return NotFound(id);
            }

            return Ok(item);
        }

        /// <summary>
        /// Создать событие
        /// </summary>
        /// <param name="data">Данные через body:
        /// Title - string,
        /// StartAt - DateTime,
        /// EndAt - DateTime,
        /// Description - string,
        /// </param>
        /// <response code="201">Событие успешно создано</response>
        [HttpPost]
        public IActionResult CreateEvent([FromBody] CreateEventRequest data)
        {
            var created = _eventService.CreateEvent(
                data.Title,
                data.StartAt,
                data.EndAt,
                data.Description);

            return Created($"/{created.Id}", created);
        }

        /// <summary>
        /// Обновить событие целиком
        /// </summary>
        /// <param name="id">Идентификатор события, Guid</param>
        /// <param name="data">Данные через body:
        /// Title - string,
        /// StartAt - DateTime,
        /// EndAt - DateTime,
        /// Description - string,
        /// </param>
        /// <response code="200">Событие успешно обновлено</response>
        /// <response code="404">Событие не найдено</response>
        [HttpPut("{id}")]
        public IActionResult UpdateEvent([FromRoute] Guid id, [FromBody] UpdateEventRequest data)
        {
            var toUpdate = new Event()
            {
                Id = id,
                Title = data.Title,
                Description = data.Description,
                StartAt = data.StartAt,
                EndAt = data.EndAt,
            };

            if (!_eventService.TryUpdateEvent(toUpdate))
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// Удалить событие
        /// </summary>
        /// <param name="id">Идентификатор события, Guid</param>
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
