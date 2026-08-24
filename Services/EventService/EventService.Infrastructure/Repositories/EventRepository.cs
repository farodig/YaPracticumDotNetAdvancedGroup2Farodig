using EventService.Application.Abstractions;
using EventService.Domain.Entities;
using EventService.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EventService.Infrastructure.Repositories
{
    public class EventRepository(EventDbContext dbContext) : IEventRepository
    {
        private readonly EventDbContext _dbContext = dbContext;

        public async Task<IEnumerable<Event>> GetEventsAsync(
            int page,
            int pageSize,
            string? title = null,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken cts = default)
        {
            return await _dbContext.Events
                .AsQueryable()
                .FilterByTitle(title)
                .FilterByFrom(from)
                .FilterByTo(to)
                .Pagination(page, pageSize)
                .ToListAsync(cts);
        }

        public async Task<Event?> GetAsync(Guid id, CancellationToken cts = default) => await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id, cts);

        public async Task CreateAsync(Event item, CancellationToken cts = default)
        {
            await _dbContext.Events.AddAsync(item, cts);
            await _dbContext.SaveChangesAsync(cts);
        }

        public async Task<int> TryUpdateAsync(Event item, CancellationToken cts = default)
        {
            var existing = await _dbContext.Events.FindAsync([item.Id], cts);
            if (existing == null) return 0;

            _dbContext.Entry(existing).CurrentValues.SetValues(item);
            return await _dbContext.SaveChangesAsync(cts);
        }

        public async Task<int> TryRemoveAsync(Guid id, CancellationToken cts = default)
        {
            var toDelete = await _dbContext.Events
                .Where(a => a.Id == id)
                .ToArrayAsync(cts);
            if (toDelete.Length == 0) return 0;

            _dbContext.Events.RemoveRange(toDelete);
            return await _dbContext.SaveChangesAsync(cts);
        }

        public async Task<bool> IsInboxDublicatedEvent(Guid id, CancellationToken cts = default)
        {
            try
            {
                await _dbContext.InboxMessages.AddAsync(new InboxMessage
                {
                    Id = id,
                    ReceivedAt = DateTime.Now,
                }, cts);
                await _dbContext.SaveChangesAsync(cts);
                return false;
            }
            catch
            {
                // Любая причина будь то недоступная бд, будь то повторная запись
                return true;
            }
        }
    }
}
