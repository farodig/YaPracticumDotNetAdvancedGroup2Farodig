using Application.Models.Responses;
using Domain.Entities;

namespace Application.Models.Builders
{
    internal static class PersonBuilder
    {
        internal static PersonResponse BuildPersonResponse(this Person data) => new()
        {
            Id = data.Id,
            Login = data.Login,
            Role = data.Role,
        };
    }
}
