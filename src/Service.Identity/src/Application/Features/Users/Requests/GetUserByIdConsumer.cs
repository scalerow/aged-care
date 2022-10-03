using Giantnodes.Service.Identity.Abstractions.Users.Requests;
using Giantnodes.Service.Identity.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Identity.Application.Features.Users.Requests
{
    public class GetUserByIdConsumer : IConsumer<GetUserByIdRequest>
    {
        private readonly ApplicationDbContext _database;

        public GetUserByIdConsumer(ApplicationDbContext database)
        {
            _database = database;
        }

        public async Task Consume(ConsumeContext<GetUserByIdRequest> context)
        {
            var user = await _database.Users.FirstOrDefaultAsync(x => x.Id == context.Message.UserId, context.CancellationToken);
            if (user == null)
            {
                await context.RejectAsync<GetUserByIdRequestRejected, GetUserByIdRequestRejection>(GetUserByIdRequestRejection.NotFound);
                return;
            }

            await context.RespondAsync<GetUserByIdRequestResult>(new { UserId = user.Id, user.Email, user.GivenName, user.FamilyName });
        }
    }
}
