using Giantnodes.Service.Identity.Abstractions.Registration.Commands;
using Giantnodes.Service.Identity.Abstractions.Registration.Events;
using Giantnodes.Service.Identity.Domain.Identity;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace Giantnodes.Service.Identity.Application.Features.Registration.Commands
{
    public class CreateUserConsumer : IConsumer<CreateUserCommand>
    {
        private readonly UserManager<ApplicationUser> _manager;

        public CreateUserConsumer(UserManager<ApplicationUser> manager)
        {
            _manager = manager;
        }

        public async Task Consume(ConsumeContext<CreateUserCommand> context)
        {
            var inputs = new List<string> { context.Message.Email, context.Message.GivenName, context.Message.FamilyName };
            var strength = Zxcvbn.Core.EvaluatePassword(context.Message.Password, inputs);
            if (strength.Score < 3)
            {
                await context.RejectAsync<CreateUserCommandRejected, CreateUserCommandRejection>(CreateUserCommandRejection.PasswordTooWeak);
                return;
            }

            var user = new ApplicationUser
            {
                Email = context.Message.Email,
                UserName = context.Message.Email,
                GivenName = context.Message.GivenName,
                FamilyName = context.Message.FamilyName
            };

            var result = await _manager.CreateAsync(user, context.Message.Password);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(x => x.Code == nameof(_manager.ErrorDescriber.DuplicateEmail)))
                {
                    await context.RejectAsync<CreateUserCommandRejected, CreateUserCommandRejection>(CreateUserCommandRejection.DuplicateEmail);
                    return;
                }

                var error = result.Errors.First();
                await context.RejectAsync<CreateUserCommandRejected, CreateUserCommandRejection>(CreateUserCommandRejection.IdentityError);
                return;
            }

            var topic = KebabCaseEndpointNameFormatter.Instance.Message<SendUserEmailConfirmationCommand>();
            var endpoint = await context.GetSendEndpoint(new Uri($"queue:{topic}"));
            await endpoint.Send<SendUserEmailConfirmationCommand>(new { user.Email });

            await context.Publish<UserCreatedEvent>(new { UserId = user.Id });
            await context.RespondAsync<CreateUserCommandResult>(new { UserId = user.Id });
        }
    }
}
