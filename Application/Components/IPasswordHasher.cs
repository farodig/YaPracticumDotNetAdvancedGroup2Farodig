namespace Application.Components
{
    /// <summary>
    /// Компонент хеширования и проверки пароля
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Генерация хеша по паролю
        /// </summary>
        string GenerateHash(string password);

        /// <summary>
        /// Проверка пароля по хешу
        /// </summary>
        bool Verify(string password, string hash);
    }
}
