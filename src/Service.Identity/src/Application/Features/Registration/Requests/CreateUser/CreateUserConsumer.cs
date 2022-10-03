using Giantnodes.Service.Identity.Abstractions.Registration.Commands;
using Giantnodes.Service.Identity.Abstractions.Registration.Events;
using Giantnodes.Service.Identity.Abstractions.Registration.Requests;
using Giantnodes.Service.Identity.Domain.Identity;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace Giantnodes.Service.Identity.Application.Features.Registration
{
    public class CreateUserConsumer : IConsumer<CreateUserRequest>
    {
        private readonly UserManager<ApplicationUser> _manager;

        public CreateUserConsumer(UserManager<ApplicationUser> manager)
        {
            _manager = manager;
        }

        public async Task Consume(ConsumeContext<CreateUserRequest> context)
        {
            var inputs = new List<string> { context.Message.Email, context.Message.GivenName, context.Message.FamilyName };
            var strength = Zxcvbn.Core.EvaluatePassword(context.Message.Password, inputs);
            if (strength.Score < 3)
            {
                await context.RejectAsync<CreateUserRequestRejected, CreateUserRequestRejection>(CreateUserRequestRejection.PasswordTooWeak);
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
                    await context.RejectAsync<CreateUserRequestRejected, CreateUserRequestRejection>(CreateUserRequestRejection.DuplicateEmail);
                    return;
                }

                var error = result.Errors.First();
                await context.RejectAsync<CreateUserRequestRejected, CreateUserRequestRejection>(CreateUserRequestRejection.IdentityError);
                return;
            }

            var topic = KebabCaseEndpointNameFormatter.Instance.Message<SendEmailConfirmationCommand>();
            var endpoint = await context.GetSendEndpoint(new Uri($"queue:{topic}"));
            await endpoint.Send<SendEmailConfirmationCommand>(new { Email = user.Email });

            await context.Publish<UserCreatedEvent>(new { UserId = user.Id });
            await context.RespondAsync<CreateUserRequestResult>(new { UserId = user.Id });
        }
    }
}
