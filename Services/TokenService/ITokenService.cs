using System.Security.Claims;

namespace TokenService
{
    /// <summary>
    /// Сервис генерации токена
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Сгенерировать токен по данным пользователя
        /// </summary>
        string CreateToken(Guid personId, string role);

        /// <summary>
        /// Получить идентификатор пользователя
        /// </summary>
        Guid GetPersonId(ClaimsPrincipal user);

        /// <summary>
        /// Является ли пользователь администратором
        /// </summary>
        bool IsAdmin(ClaimsPrincipal user);
    }
}
