using BookingService.Application.Abstractions;
using BookingService.Infrastructure.DataAccess;
using BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Repositories
{
    public class BookingRepository(BookingDbContext dbContext) : IBookingRepository
    {
        private readonly BookingDbContext _dbContext = dbContext;

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

        public async Task<int> GetBookingCountAsync(Guid personId, CancellationToken cts = default)
        {
            var currnetDateTime = DateTime.Now;
            return await _dbContext.Bookings
                //.Include(b => b.Event)
                .Where(p => p.Status == BookingStatus.Confirmed)
                .Where(p => p.PersonId == personId)
                //.Where(p => currnetDateTime <= p.Event.EndAt)
                // TODO: связь с датой и временем события (Пользователь достиг лимита на количество активных броней)
                // а нужно ли вообще запрашивать из booking? вероятно нужно делать запрос из PersonDb
                // или у букинга должно быть новое состояние (завершено - BookingStatus удалено / Дата и время завершения события), подумать
                .CountAsync(cts);
        }

        public async Task CreateAsync(Booking item, CancellationToken cts = default)
        {
            await _dbContext.Bookings.AddAsync(item, cts);
            await _dbContext.SaveChangesAsync(cts);
        }

        public async Task<int> TryUpdateStatusAsync(Booking item, BookingStatus status, CancellationToken cts = default)
        {
            if (item.Status == status) return 0;

            var existing = await _dbContext.Bookings.FindAsync([item.Id], cts);
            if (existing == null) return 0;

            item.Status = status;
            item.ProcessedAt = DateTime.Now;

            _dbContext.Entry(existing).CurrentValues.SetValues(item);

            return await _dbContext.SaveChangesAsync(cts);
        }
    }
}