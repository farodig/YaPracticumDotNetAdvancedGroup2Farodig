using Domain.Entities;
using System.Security.Claims;

namespace Application.Abstractions
{
    /// <summary>
    /// Сервис генерации токена
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Сгенерировать токен по данным пользователя
        /// </summary>
        string CreateToken(Person person);

        /// <summary>
        /// Получить идентификатор пользователя
        /// </summary>
        Guid GetPersonId(ClaimsPrincipal user);

        /// <summary>
        /// Получить роль пользователя
        /// </summary>
        PersonRole GetRole(ClaimsPrincipal user);
    }
}
