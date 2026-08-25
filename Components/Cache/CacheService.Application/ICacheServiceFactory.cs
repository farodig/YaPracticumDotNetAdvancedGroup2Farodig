namespace CacheService.Application
{
    public interface ICacheServiceFactory
    {
        ICacheService<TItem> CreateCacheService<TItem>()
            where TItem : class;
    }
}
