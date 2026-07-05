using LearningWebApi.DataAccess;
using LearningWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningWebApi.Repositories
{
    internal class EventRepository(AppDbContext dbContext) : IEventRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public IQueryable<Event> GetEvents()
        {
            return _dbContext.Events.AsQueryable();
        }

        public async Task<Event?> GetAsync(Guid id, CancellationToken? cts = null) => await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id, cts ?? CancellationToken.None);

        public async Task CreateAsync(Event item, CancellationToken? cts = null)
        {
            await _dbContext.Events.AddAsync(item, cts ?? CancellationToken.None);
            await _dbContext.SaveChangesAsync(cts ?? CancellationToken.None);
        }

        public async Task<int> TryUpdateAsync(Event item, CancellationToken? cts = null)
        {
            var existing = await _dbContext.Events.FindAsync(item.Id);
            if (existing == null) return 0;

            _dbContext.Entry(existing).CurrentValues.SetValues(item);
            return await _dbContext.SaveChangesAsync(cts ?? CancellationToken.None);
        }

        public async Task<bool> TryUpdateContextAsync(Event item, CancellationToken? cts = null)
        {
            var existing = await _dbContext.Events.FindAsync(item.Id);
            if (existing == null) return false;

            _dbContext.Entry(existing).CurrentValues.SetValues(item);
            return true;
        }

        public async Task<int> TryRemoveAsync(Guid id, CancellationToken? cts = null)
        {
            var toDelete = await _dbContext.Events
                .Where(a => a.Id == id)
                .ToArrayAsync(cts ?? CancellationToken.None);
            if (toDelete.Length == 0) return 0;

            _dbContext.Events.RemoveRange(toDelete);
            return await _dbContext.SaveChangesAsync(cts ?? CancellationToken.None);
        }
    }
}
