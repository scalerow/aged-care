using Giantnodes.Service.Identity.Abstractions.Registration.Events;
using Giantnodes.Service.Identity.Abstractions.Registration.Requests;
using Giantnodes.Service.Identity.Domain.Identity;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace Giantnodes.Service.Identity.Application.Features.Registration.Requests
{
    public class ConfirmEmailConsumer : IConsumer<ConfirmEmailRequest>
    {
        private readonly UserManager<ApplicationUser> _manager;

        public ConfirmEmailConsumer(UserManager<ApplicationUser> manager)
        {
            _manager = manager;
        }

        public async Task Consume(ConsumeContext<ConfirmEmailRequest> context)
        {
            var user = await _manager.FindByEmailAsync(context.Message.Email);
            if (user == null)
            {
                await context.RejectAsync<ConfirmEmailRequestRejected, ConfirmEmailRequestRejection>(ConfirmEmailRequestRejection.NOT_FOUND);
                return;
            }

            if (user.EmailConfirmed)
            {
                await context.RejectAsync<ConfirmEmailRequestRejected, ConfirmEmailRequestRejection>(ConfirmEmailRequestRejection.ALREADY_CONFIRMED);
                return;
            }

            var result = await _manager.ConfirmEmailAsync(user, context.Message.Token);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                await context.RejectAsync<ConfirmEmailRequestRejected, ConfirmEmailRequestRejection>(ConfirmEmailRequestRejection.IDENTITY_ERROR, error.Description);
                return;
            }

            await context.Publish<UserConfirmedEmailEvent>(new { UserId = user.Id, user.Email });
            await context.RespondAsync<ConfirmEmailRequestResult>(new { UserId = user.Id, user.Email });
        }
    }
}
