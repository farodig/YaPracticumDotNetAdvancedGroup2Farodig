namespace Domain.Exceptions
{
    /// <summary>
    /// Пользователь не найден или не совпадает логин и пароль
    /// </summary>
    public class WrongLoginOrPasswordException() : Exception("Wrong login or password")
    {
    }
}
