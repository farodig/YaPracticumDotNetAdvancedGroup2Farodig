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
            try
            {
                return await _dbContext.Events
                    .Where(a => a.Id == item.Id)
                    .ExecuteUpdateAsync(setters => setters
                    .SetProperty(a => a.Title, item.Title)
                    .SetProperty(a => a.Description, item.Description)
                    .SetProperty(a => a.StartAt, item.StartAt)
                    .SetProperty(a => a.EndAt, item.EndAt)
                    .SetProperty(a => a.TotalSeats, item.TotalSeats)
                    .SetProperty(a => a.AvailableSeats, item.AvailableSeats)
                    , cts ?? CancellationToken.None);
            }
            finally
            {
                _dbContext.ChangeTracker.Clear();
            }
        }

        public async Task<int> TryRemoveAsync(Guid id, CancellationToken? cts = null)
        {
            try
            {
                return await _dbContext.Events
                    .Where(a => a.Id == id)
                    .ExecuteDeleteAsync(cts ?? CancellationToken.None);
            }
            finally
            {
                _dbContext.ChangeTracker.Clear();
            }
        }
    }
}
