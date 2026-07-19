using Domain.Entities;

namespace Application.Services.TokenService
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
    }
}
