using System.ComponentModel.DataAnnotations;

namespace Application.Models.Requests
{
    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    public class LoginPersonRequest
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
    }
}
