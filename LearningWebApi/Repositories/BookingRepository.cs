using LearningWebApi.DataAccess;
using LearningWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningWebApi.Repositories
{
    internal class BookingRepository(AppDbContext dbContext) : IBookingRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public IQueryable<Booking> GetBookings()
        {
            return _dbContext.Bookings.AsQueryable();
        }

        public async Task<Booking?> GetAsync(Guid id, CancellationToken? cts = null)
        {
            return await _dbContext.Bookings.FirstOrDefaultAsync(e => e.Id == id, cts ?? CancellationToken.None);
        }

        public async Task CreateAsync(Booking item, CancellationToken? cts = null)
        {
            await _dbContext.Bookings.AddAsync(item, cts ?? CancellationToken.None);
            await _dbContext.SaveChangesAsync(cts ?? CancellationToken.None);
        }

        public async Task<int> TryUpdateAsync(Booking item, CancellationToken? cts = null)
        {
            var existing = await _dbContext.Bookings.FindAsync(item.Id);
            if (existing == null) return 0;

            _dbContext.Entry(existing).CurrentValues.SetValues(item);
            return await _dbContext.SaveChangesAsync(cts ?? CancellationToken.None);
        }

        public async Task<int> TryRemoveAsync(Guid id, CancellationToken? cts = null)
        {
            var toDelete = await _dbContext.Bookings
                .Where(a => a.Id == id)
                .ToArrayAsync(cts ?? CancellationToken.None);
            if (toDelete.Length == 0) return 0;

            _dbContext.Bookings.RemoveRange(toDelete);
            return await _dbContext.SaveChangesAsync(cts ?? CancellationToken.None);
        }
    }
}