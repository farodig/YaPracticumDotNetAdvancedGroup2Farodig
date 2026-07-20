using Domain.Entities;

namespace Application.Models.Responses
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class PersonResponse
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Роль пользователя
        /// </summary>
        public PersonRole Role { get; set; }
    }
}
