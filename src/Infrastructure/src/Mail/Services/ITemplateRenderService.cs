using Giantnodes.Infrastructure.Mail.Abstractions;

namespace Giantnodes.Infrastructure.Mail.Services
{
    public interface ITemplateRenderService
    {
        Task<string> RenderAsync<T>(T template)
             where T : EmailTemplate;
    }
}
