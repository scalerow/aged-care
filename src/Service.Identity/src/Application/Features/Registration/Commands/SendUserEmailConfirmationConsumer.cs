using Giantnodes.Infrastructure.Mail.Services;
using Giantnodes.Service.Identity.Abstractions.Registration.Commands;
using Giantnodes.Service.Identity.Domain.Identity;
using Giantnodes.Service.Identity.Mail.Templates;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using MimeKit;

namespace Giantnodes.Service.Identity.Application.Features.Registration.Commands
{
    public class SendUserEmailConfirmationConsumer : IConsumer<SendUserEmailConfirmationCommand>
    {
        private readonly UserManager<ApplicationUser> _manager;
        private readonly IEmailService _email;

        public SendUserEmailConfirmationConsumer(UserManager<ApplicationUser> manager, IEmailService email)
        {
            _manager = manager;
            _email = email;
        }

        public async Task Consume(ConsumeContext<SendUserEmailConfirmationCommand> context)
        {
            var user = await _manager.FindByEmailAsync(context.Message.Email);
            if (user == null)
            {
                await context.RejectAsync<SendUserEmailConfirmationCommandRejected, SendUserEmailConfirmationCommandRejection>(SendUserEmailConfirmationCommandRejection.NotFound);
                return;
            }

            if (user.EmailConfirmed)
            {
                await context.RejectAsync<SendUserEmailConfirmationCommandRejected, SendUserEmailConfirmationCommandRejection>(SendUserEmailConfirmationCommandRejection.AlreadyConfirmed);
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
