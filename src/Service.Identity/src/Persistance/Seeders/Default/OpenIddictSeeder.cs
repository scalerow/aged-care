using Giantnodes.Service.Identity.Persistence.Seeders.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Giantnodes.Service.Identity.Persistence.Seeders.Default
{
    public class OpenIddictSeeder : IDatabaseSeeder
    {
        private readonly string ClientId = "web";

        public async Task SeedAsync(ApplicationDbContext database, IServiceProvider provider, CancellationToken cancellation = default)
        {
            var openiddict = provider.GetRequiredService<IOpenIddictApplicationManager>();
            var client = await openiddict.FindByClientIdAsync(ClientId, cancellation);

            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = ClientId,
                DisplayName = "Giantnodes Web Application",
                ConsentType = ConsentTypes.Systematic,
                Type = ClientTypes.Public,
                Permissions = {
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Logout,

                    Permissions.GrantTypes.Password,
                    Permissions.GrantTypes.RefreshToken,

                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles
                }
            };

            if (client == null)
                await openiddict.CreateAsync(descriptor, cancellation);

            if (client != null)
                await openiddict.UpdateAsync(client, descriptor, cancellation);
        }
    }
}
