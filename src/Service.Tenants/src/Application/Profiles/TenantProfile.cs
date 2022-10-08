using AutoMapper;
using Giantnodes.Service.Tenants.Abstractions.Views.Tenants;
using Giantnodes.Service.Tenants.Domain.Entities;

namespace Giantnodes.Service.Tenants.Application.Profiles
{
    public class TenantProfile : Profile
    {
        public TenantProfile()
        {
            CreateMap<Tenant, TenantView>();
        }
    }
}
