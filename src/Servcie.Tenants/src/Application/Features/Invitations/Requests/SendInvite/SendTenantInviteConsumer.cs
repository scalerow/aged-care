using Giantnodes.Infrastructure.Mail.Services;
using Giantnodes.Service.Identity.Abstractions.Users.Requests;
using Giantnodes.Service.Identity.Mail.Templates;
using Giantnodes.Service.Tenants.Abstractions.Invitations.Requests;
using Giantnodes.Service.Tenants.Domain.Entities;
using Giantnodes.Service.Tenants.Persistance;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace Giantnodes.Service.Tenants.Application.Features.Invitations.Requests
{
    public class SendTenantInviteConsumer : IConsumer<SendTenantInviteRequest>
    {
        private readonly ApplicationDbContext _database;
        private readonly IRequestClient<GetUserByIdRequest> _client;
        private readonly IEmailService _email;

        public SendTenantInviteConsumer(ApplicationDbContext database, IRequestClient<GetUserByIdRequest> client, IEmailService email)
        {
            _database = database;
            _client = client;
            _email = email;
        }

        public async Task Consume(ConsumeContext<SendTenantInviteRequest> context)
        {
            var tenant = await _database
                .Tenants
                .Include(x => x.Users.Where(u => u.Id == context.Message.UserId))
                .FirstOrDefaultAsync(x => x.Id == context.Message.TenantId, context.CancellationToken);

            if (tenant == null)
            {
                await context.RejectAsync<SendTenantInviteRequestRejected, SendTenantInviteRequestRejection>(SendTenantInviteRequestRejection.TenantNotFound);
                return;
            }

            if (tenant.Users.Any(x => x.UserId == context.Message.UserId))
            {
                await context.RejectAsync<SendTenantInviteRequestRejected, SendTenantInviteRequestRejection>(SendTenantInviteRequestRejection.AlreadyJoined);
                return;
            }

            Response response = await _client.GetResponse<GetUserByIdRequestResult, GetUserByIdRequestRejected>(new { context.Message.UserId });
            var user = response switch
            {
                (_, GetUserByIdRequestResult result) => result,
                (_, GetUserByIdRequestRejected rejected) => null,
                _ => throw new InvalidOperationException()
            };

            if (user == null)
            {
                await context.RejectAsync<SendTenantInviteRequestRejected, SendTenantInviteRequestRejection>(SendTenantInviteRequestRejection.UserNotFound);
                return;
            }

            var invite = await _database
                .TenantInvites
                .FirstOrDefaultAsync(x => x.UserId == context.Message.UserId && x.TenantId == context.Message.TenantId, context.CancellationToken);

            if (invite != null)
            {
                // prevent sending too many invitations too quickly
                if (invite.LastSentAt.HasValue && DateTime.UtcNow <= invite.LastSentAt.Value.AddMinutes(TenantInvite.ResendMinutes))
                {
                    await context.RejectAsync<SendTenantInviteRequestRejected, SendTenantInviteRequestRejection>(SendTenantInviteRequestRejection.AlreadySent);
                    return;
                }

                // reset the expirary and code to invalidate existing invitation links that have been sent
                invite.Code = Guid.NewGuid();
                invite.ExpiresAt = DateTime.UtcNow.AddHours(TenantInvite.LifetimeHours);
            }

            if (invite == null)
            {
                invite = new TenantInvite { TenantId = tenant.Id, UserId = user.UserId, Code = Guid.NewGuid() };
                _database.TenantInvites.Add(invite);
                await _database.SaveChangesAsync(context.CancellationToken);
            }

            var template = new TenentInvitationTemplate
            {
                Code = invite.Code,
                TenantName = tenant.Name,
                UserEmail = user.Email,
                UserFullName = $"{user.GivenName} {user.FamilyName}",
            };

            var address = new MailboxAddress($"{user.GivenName} {user.FamilyName}", user.Email);
            await _email.SendEmailAsync(template, address, context.CancellationToken);

            invite.LastSentAt = DateTime.UtcNow;
            await _database.SaveChangesAsync(context.CancellationToken);
            await context.RespondAsync<SendTenantInviteRequestResult>(new { TenantId = tenant.Id, user.UserId });
        }
    }
}
