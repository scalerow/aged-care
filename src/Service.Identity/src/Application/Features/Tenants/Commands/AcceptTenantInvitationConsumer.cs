using Giantnodes.Service.Identity.Abstractions.Tenants.Commands;
using Giantnodes.Service.Identity.Domain.Entities;
using Giantnodes.Service.Identity.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Identity.Application.Features.Tenants.Commands
{
    public class AcceptTenantInvitationConsumer : IConsumer<AcceptTenantInvitationCommand>
    {
        private readonly ApplicationDbContext _database;

        public AcceptTenantInvitationConsumer(ApplicationDbContext database)
        {
            this._database = database;
        }

        public async Task Consume(ConsumeContext<AcceptTenantInvitationCommand> context)
        {
            var invite = await _database.UserTenantInvites.FirstOrDefaultAsync(x => x.Token == context.Message.Token, context.CancellationToken);
            if (invite == null)
            {
                await context.RejectAsync<AcceptTenantInvitationCommandRejected, AcceptTenantInvitationCommandRejection>(AcceptTenantInvitationCommandRejection.NotFound);
                return;
            }

            if (DateTime.UtcNow > invite.ExpiresAt)
            {
                await context.RejectAsync<AcceptTenantInvitationCommandRejected, AcceptTenantInvitationCommandRejection>(AcceptTenantInvitationCommandRejection.Expired);

                _database.UserTenantInvites.Remove(invite);
                await _database.SaveChangesAsync(context.CancellationToken);
                return;
            }

            var connection = new UserTenant { UserId = invite.UserId, TenantId = invite.TenantId };
            _database.UserTenants.Add(connection);
            _database.UserTenantInvites.Remove(invite);
            await _database.SaveChangesAsync(context.CancellationToken);

            await context.RespondAsync<AcceptTenantInvitationCommandResult>(new { UserTenantId = connection.Id });
        }
    }
}
