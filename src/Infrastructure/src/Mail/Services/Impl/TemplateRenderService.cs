using Giantnodes.Infrastructure.Mail.Abstractions;
using Mjml.Net;
using Razor.Templating.Core;

namespace Giantnodes.Infrastructure.Mail.Services
{
    public class TemplateRenderService : ITemplateRenderService
    {
        public async Task<string> RenderAsync<T>(T template)
            where T : EmailTemplate
        {
            var razor = await RazorTemplateEngine.RenderAsync(template.Path, template);

            return template.Engine switch
            {
                TemplateRenderEngine.Mjml => new MjmlRenderer().Render(razor).Html,
                _ => razor
            };
        }
    }
}
