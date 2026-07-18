namespace Domain.Entities
{
    /// <summary>
    /// Пользователь (человек)
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Хеш пароля
        /// </summary>
        public byte[] PasswordHash { get; set; } = [];

        /// <summary>
        /// Роль
        /// </summary>
        public PersonRole Role { get; set; }
    }
}
