namespace SharedContracts.Abstractions
{
    public interface IEvent
    {
        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public Guid Id { get; set; }
    }
}
