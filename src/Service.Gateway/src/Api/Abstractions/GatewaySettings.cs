namespace Giantnodes.Service.Gateway.Api.Abstractions
{
    public class GatewaySettings
    {
        public string Name { get; init; } = null!;

        public ICollection<GatewayDomainSettings> Domains { get; init; } = Array.Empty<GatewayDomainSettings>();
    }

    public class GatewayDomainSettings
    {
        public string Name { get; init; } = null!;

        public string SchemaConnection { get; init; } = null!;
    }
}
