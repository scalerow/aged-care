namespace Giantnodes.Service.Tenants.Abstractions.Views.Tenants.Partials
{
    internal interface ITenant
    {
        public Guid Id { get; init; }

        public string Name { get; init; }
    }
}
