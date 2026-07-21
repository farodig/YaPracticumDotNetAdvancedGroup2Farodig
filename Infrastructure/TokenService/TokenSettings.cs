using System.ComponentModel.DataAnnotations;

namespace Application.Services.TokenService
{
    /// <summary>
    /// Настройки генерации токена
    /// </summary>
    public class TokenSettings
    {
        /// <summary>
        /// Издатель
        /// </summary>
        [Required]
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Аудитория
        /// </summary>
        [Required]
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Время жизни, мин
        /// </summary>
        [Required]
        public int ExpirationMin { get; set; } = 60;

        /// <summary>
        /// Закрытый ключ
        /// </summary>
        [Required]
        public string Secret { get; set; } = string.Empty;
    }
}
