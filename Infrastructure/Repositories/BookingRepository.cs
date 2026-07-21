using Application.Abstractions;
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

        public async Task<Booking?> GetWithPersonAsync(Guid id, CancellationToken cts = default)
        {
            return await _dbContext.Bookings
                .Include(a => a.Person)
                .FirstOrDefaultAsync(e => e.Id == id, cts);
        }

        public async Task<int> GetBookingCountAsync(Guid personId, CancellationToken cts = default)
        {
            var currnetDateTime = DateTime.Now;
            return await _dbContext.Bookings
                .Include(b => b.Event)
                .Where(p => p.PersonId == personId)
                .Where(p => currnetDateTime <= p.Event.EndAt)
                .CountAsync(cts);
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

        public async Task<int> TryRemoveAsync(Booking data, CancellationToken cts = default)
        {
            data.Status = BookingStatus.Cancelled;
            return await TryUpdateAsync(data, cts);
        }
    }
}