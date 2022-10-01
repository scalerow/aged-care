using Giantnodes.Infrastructure.Mail.Services;
using Giantnodes.Service.Identity.Abstractions.Registration.Commands;
using Giantnodes.Service.Identity.Domain.Identity;
using Giantnodes.Service.Identity.Mail.Templates;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using MimeKit;

namespace Giantnodes.Service.Identity.Application.Features.Registration
{
    public class SendEmailConfirmationConsumer : IConsumer<SendEmailConfirmationCommand>
    {
        private readonly UserManager<ApplicationUser> _manager;
        private readonly IEmailService _email;

        public SendEmailConfirmationConsumer(UserManager<ApplicationUser> manager, IEmailService email)
        {
            _manager = manager;
            _email = email;
        }

        public async Task Consume(ConsumeContext<SendEmailConfirmationCommand> context)
        {
            var user = await _manager.FindByEmailAsync(context.Message.Email);
            if (user == null)
            {
                await context.RejectAsync<SendEmailConfirmationCommandRejected, SendEmailConfirmationCommandRejection>(SendEmailConfirmationCommandRejection.NOT_FOUND);
                return;
            }

            if (user.EmailConfirmed)
            {
                await context.RejectAsync<SendEmailConfirmationCommandRejected, SendEmailConfirmationCommandRejection>(SendEmailConfirmationCommandRejection.ALREADY_CONFIRMED);
                return;
            }

            var code = await _manager.GenerateEmailConfirmationTokenAsync(user);
            var template = new EmailConfirmationTemplate
            {
                Code = code,
                Email = user.Email,
                FullName = user.FullName,
                VerifyLink = $"/register/verify"
            };

            var address = new MailboxAddress(user.FullName, user.Email);
            await _email.SendEmailAsync(template, address, context.CancellationToken);
        }
    }
}
