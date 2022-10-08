using Giantnodes.Service.Identity.Abstractions.Registration.Commands;
using Giantnodes.Service.Identity.Abstractions.Registration.Events;
using Giantnodes.Service.Identity.Domain.Identity;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace Giantnodes.Service.Identity.Application.Features.Registration.Commands
{
    public class ConfirmUserEmailConsumer : IConsumer<ConfirmUserEmailCommand>
    {
        private readonly UserManager<ApplicationUser> _manager;

        public ConfirmUserEmailConsumer(UserManager<ApplicationUser> manager)
        {
            _manager = manager;
        }

        public async Task Consume(ConsumeContext<ConfirmUserEmailCommand> context)
        {
            var user = await _manager.FindByEmailAsync(context.Message.Email);
            if (user == null)
            {
                await context.RejectAsync<ConfirmUserEmailCommandRejected, ConfirmUserEmailCommandRejection>(ConfirmUserEmailCommandRejection.NotFound);
                return;
            }

            if (user.EmailConfirmed)
            {
                await context.RejectAsync<ConfirmUserEmailCommandRejected, ConfirmUserEmailCommandRejection>(ConfirmUserEmailCommandRejection.AlreadyConfirmed);
                return;
            }

            var result = await _manager.ConfirmEmailAsync(user, context.Message.Token);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                await context.RejectAsync<ConfirmUserEmailCommandRejected, ConfirmUserEmailCommandRejection>(ConfirmUserEmailCommandRejection.IdentityError, error.Description);
                return;
            }

            await context.Publish<UserConfirmedEmailEvent>(new { UserId = user.Id, user.Email });
            await context.RespondAsync<ConfirmUserEmailCommandResult>(new { UserId = user.Id });
        }
    }
}
