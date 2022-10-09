using MassTransit;

namespace Giantnodes.Infrastructure.MassTransit.Session
{
    public class UserSessionFilter<TMessage> : IFilter<SendContext<TMessage>>
        where TMessage : class
    {
        public UserSessionFilter()
        {

        }

        public void Probe(ProbeContext context) { }

        public async Task Send(SendContext<TMessage> context, IPipe<SendContext<TMessage>> next)
        {
            context.Headers.Set("UserId", 123);
            await next.Send(context);
        }
    }
}
