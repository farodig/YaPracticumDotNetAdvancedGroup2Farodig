using PersonService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PersonService.Infrastructure.DataAccess
{
    public sealed class PersonDbContext(DbContextOptions<PersonDbContext> options) : DbContext(options)
    {
        public DbSet<Person> Persons => Set<Person>();
    }
}
