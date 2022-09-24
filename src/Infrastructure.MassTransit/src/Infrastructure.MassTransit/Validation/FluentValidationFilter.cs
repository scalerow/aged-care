using FluentValidation;
using MassTransit;

namespace Giantnodes.Infrastructure.Masstransit.Validation
{
    public class FluentValidationFilter<TMessage> : IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        private readonly IValidator<TMessage>? _validator;

        public FluentValidationFilter(IEnumerable<IValidator<TMessage>>? validator)
        {
            // using an IEnumerable to prevent messages with no IValidator to resolve as null
            // preventing InvalidOperationExceptions, as not every incoming message requires validation.
            _validator = validator?.FirstOrDefault();
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("fluent-validation-filter");
        }

        public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            if (_validator == null)
            {
                await next.Send(context);
                return;
            }

            var message = context.Message;
            var result = await _validator.ValidateAsync(message, context.CancellationToken);
            if (result.IsValid)
            {
                await next.Send(context);
                return;
            }

            await context.RespondAsync(result.ToFault());
        }
    }
}
