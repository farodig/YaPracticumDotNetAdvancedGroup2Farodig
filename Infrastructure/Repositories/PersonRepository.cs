using Application.Repositories;
using Domain.Entities;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PersonRepository(AppDbContext dbContext): IPersonRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<Person?> GetByLoginAsync(string login, CancellationToken cts = default) => await _dbContext.Persons.FirstOrDefaultAsync(p => p.Login == login, cts);

        public async Task CreateAsync(Person person, CancellationToken cts = default)
        {
            await _dbContext.Persons.AddAsync(person, cts);
            await _dbContext.SaveChangesAsync(cts);
        }
    }
}
