using Giantnodes.Infrastructure.Exceptions;
using Giantnodes.Infrastructure.Masstransit.Validation;
using Giantnodes.Service.Identity.Abstractions.Registration.Commands;
using Giantnodes.Service.Identity.Domain.Identity;
using Giantnodes.Service.Identity.Persistence;
using MassTransit;

namespace Giantnodes.Service.Identity.Api.Resolvers.Registration.Mutations
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class RegisterUserRes
    {
        [UseFirstOrDefault]
        public async Task<IQueryable<ApplicationUser>> RegisterUser(
            [Service] ApplicationDbContext database,
            [Service] IRequestClient<CreateUserCommand> client,
            CreateUserCommand command,
            CancellationToken cancellation
        )
        {
            Response response = await client.GetResponse<CreateUserCommandResult, CreateUserCommandRejected, ValidationFault>(command, cancellation);
            return response switch
            {
                (_, CreateUserCommandResult result) => database.Users.Where(x => x.Id == result.UserId),
                (_, CreateUserCommandRejected error) => throw new DomainException<CreateUserCommandRejected>(error),
                (_, ValidationFault error) => throw new DomainException<ValidationFault>(error),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
