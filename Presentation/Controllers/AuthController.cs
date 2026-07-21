using Application.Models.Requests;
using Application.Services.PersonService;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// Конечная точка авторизации пользователя
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IPersonService personService) : ControllerBase
    {
        private readonly IPersonService _personService = personService;

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="data">Тело запроса</param>
        /// <response code="200">Пользователь зарегистрирован</response>
        /// <returns>Токен</returns>
        [HttpPost("/auth/register")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK, "application/json")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterPersonRequest data)
        {
            await _personService.RegisterAsync(data.Login, data.Password, data.Role, HttpContext.RequestAborted);
            return NoContent();
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <param name="data">Тело запроса</param>
        /// <response code="200">Пользователь авторизован</response>
        /// <returns>Токен</returns>
        [HttpPost("/auth/login")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK, "application/json")]
        public async Task<ActionResult<string>> Login([FromBody] LoginPersonRequest data)
        {
            var token = await _personService.LoginAsync(data.Login, data.Password, HttpContext.RequestAborted);
            return Ok(token);
        }
    }
}
