using Bogus;
using Giantnodes.Service.Tenants.Domain.Entities;

namespace Giantnodes.Service.Tenants.Shared.Tests.Bogus
{
    public class TenantFaker : Faker<Tenant>
    {
        public TenantFaker()
        {
            RuleFor(p => p.Id, f => f.Random.Uuid());
            RuleFor(p => p.Name, f => f.Company.CompanyName());
        }
    }
}
