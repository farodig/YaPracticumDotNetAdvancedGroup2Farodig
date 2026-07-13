using Application.Repositories;
using Domain.Entities;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BookingRepository(AppDbContext dbContext) : IBookingRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<IEnumerable<Booking>> GetBookingsByStatus(BookingStatus status, CancellationToken cts = default)
        {
            return await _dbContext.Bookings
                .Where(a => a.Status == status)
                .ToListAsync(cts);
        }

        public async Task<Booking?> GetAsync(Guid id, CancellationToken cts = default)
        {
            return await _dbContext.Bookings.FirstOrDefaultAsync(e => e.Id == id, cts);
        }

        public async Task CreateAsync(Booking item, CancellationToken cts = default)
        {
            await _dbContext.Bookings.AddAsync(item, cts);
            await _dbContext.SaveChangesAsync(cts);
        }

        public async Task<int> TryUpdateAsync(Booking item, CancellationToken cts = default)
        {
            var existing = await _dbContext.Bookings.FindAsync([item.Id], cts);
            if (existing == null) return 0;

            _dbContext.Entry(existing).CurrentValues.SetValues(item);
            return await _dbContext.SaveChangesAsync(cts);
        }

        public async Task<int> TryRemoveAsync(Guid id, CancellationToken cts = default)
        {
            var toDelete = await _dbContext.Bookings
                .Where(a => a.Id == id)
                .ToArrayAsync(cts);
            if (toDelete.Length == 0) return 0;

            _dbContext.Bookings.RemoveRange(toDelete);
            return await _dbContext.SaveChangesAsync(cts);
        }
    }
}