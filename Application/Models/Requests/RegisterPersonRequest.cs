using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Models.Requests
{
    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    public class RegisterPersonRequest
    {
        /// <summary>
        /// Логин
        /// </summary>
        [Required]
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Роль
        /// </summary>
        public PersonRole Role { get; set; } = PersonRole.User;
    }
}
