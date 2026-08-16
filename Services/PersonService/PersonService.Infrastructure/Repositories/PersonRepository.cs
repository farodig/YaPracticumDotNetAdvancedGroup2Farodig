using PersonService.Application;
using PersonService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PersonService.Infrastructure.DataAccess;

namespace PersonService.Infrastructure.Repositories
{
    public class PersonRepository(PersonDbContext dbContext): IPersonRepository
    {
        private readonly PersonDbContext _dbContext = dbContext;

        public async Task<Person?> GetByLoginAsync(string login, CancellationToken cts = default) => await _dbContext.Persons.FirstOrDefaultAsync(p => p.Login == login, cts);

        public async Task CreateAsync(Person person, CancellationToken cts = default)
        {
            await _dbContext.Persons.AddAsync(person, cts);
            await _dbContext.SaveChangesAsync(cts);
        }
    }
}
