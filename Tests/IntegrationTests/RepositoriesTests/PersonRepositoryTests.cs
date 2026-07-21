using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using static IntegrationTests.Helpers.EntityFactory;

namespace IntegrationTests.RepositoriesTests
{
    [Collection("SequentialTests")]
    public class PersonRepositoryTests : ADockerDB
    {
        [Fact(DisplayName = "01. Проверка уникальности логина")]
        public async Task CreateNotUnicueTest()
        {
            // Arrange
            var uniqueName = "TheSameName";

            await using var context = CreateContext();
            var person = CreatePerson(login: uniqueName);
            context.Persons.Add(person);
            await context.SaveChangesAsync();

            var repository = CreatePersonRepository(context);

            // Assert
            await Assert.ThrowsAsync<DbUpdateException>( async () =>
            {
                // Action
                var personTheSameLogin = CreatePerson(login: uniqueName);
                await repository.CreateAsync(personTheSameLogin);
            });
        }
    }
}
