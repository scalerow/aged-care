using Giantnodes.Infrastructure.Mail.Services;
using Giantnodes.Service.Identity.Abstractions.Tenants.Commands;
using Giantnodes.Service.Identity.Domain.Entities;
using Giantnodes.Service.Identity.Mail.Templates;
using Giantnodes.Service.Identity.Persistence;
using Giantnodes.Service.Tenants.Abstractions.Messages.Tenants.Queries;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace Giantnodes.Service.Identity.Application.Features.Tenants.Commands
{
    public class SendTenantInvitationConsumer : IConsumer<SendTenantInvitationCommand>
    {
        private readonly ApplicationDbContext _database;
        private readonly IEmailService _email;
        private readonly IRequestClient<GetTenantByIdQuery> _client;

        public SendTenantInvitationConsumer(ApplicationDbContext database, IEmailService email, IRequestClient<GetTenantByIdQuery> client)
        {
            this._database = database;
            this._email = email;
            this._client = client;
        }

        public async Task Consume(ConsumeContext<SendTenantInvitationCommand> context)
        {
            var user = await _database
                .Users
                .Include(x => x.Tenants.Where(t => t.Id == context.Message.TenantId))
                .FirstOrDefaultAsync(x => x.Id == context.Message.UserId, context.CancellationToken);

            if (user == null)
            {
                await context.RejectAsync<SendTenantInvitationCommandRejected, SendTenantInvitationCommandRejection>(SendTenantInvitationCommandRejection.UserNotFound);
                return;
            }

            if (user.Tenants.Any(x => x.TenantId == context.Message.TenantId))
            {
                await context.RejectAsync<SendTenantInvitationCommandRejected, SendTenantInvitationCommandRejection>(SendTenantInvitationCommandRejection.UserAlreadyPresent);
                return;
            }

            Response response = await _client.GetResponse<GetTenantByIdQueryResult, GetTenantByIdQueryRejected>(new { context.Message.TenantId });
            var tenant = response switch
            {
                (_, GetTenantByIdQueryResult result) => result.Tenant,
                _ => null
            };

            if (tenant == null)
            {
                await context.RejectAsync<SendTenantInvitationCommandRejected, SendTenantInvitationCommandRejection>(SendTenantInvitationCommandRejection.TenantNotFound);
                return;
            }

            var invite = await _database.UserTenantInvites.FirstOrDefaultAsync(x => x.UserId == user.Id && x.TenantId == tenant.Id, context.CancellationToken);
            if (invite != null)
            {
                // prevent sending too many invitations too quickly
                if (invite.LastSentAt.HasValue && DateTime.UtcNow <= invite.LastSentAt.Value.AddMinutes(UserTenantInvite.ResendMinutes))
                {
                    await context.RejectAsync<SendTenantInvitationCommandRejected, SendTenantInvitationCommandRejection>(SendTenantInvitationCommandRejection.AlreadySent);
                    return;
                }

                // reset the expirary and code to invalidate existing invitation links that have been sent
                invite.Token = Guid.NewGuid();
                invite.ExpiresAt = DateTime.UtcNow.AddHours(UserTenantInvite.LifetimeHours);
            }

            if (invite == null)
            {
                invite = new UserTenantInvite { TenantId = tenant.Id, UserId = user.Id, Token = Guid.NewGuid() };
                _database.UserTenantInvites.Add(invite);
                await _database.SaveChangesAsync(context.CancellationToken);
            }

            var template = new TenentInvitationTemplate { Recipient = user, Tenant = tenant.Name, Token = invite.Token };
            var address = new MailboxAddress(user.FullName, user.Email);
            await _email.SendEmailAsync(template, address, context.CancellationToken);

            invite.LastSentAt = DateTime.UtcNow;
            await _database.SaveChangesAsync(context.CancellationToken);
            await context.RespondAsync<SendTenantInvitationCommandResult>(new { UserTenantInviteId = invite.Id });
        }
    }
}
