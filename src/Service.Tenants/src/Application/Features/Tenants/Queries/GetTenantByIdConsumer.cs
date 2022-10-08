using AutoMapper;
using Giantnodes.Service.Tenants.Abstractions.Messages.Tenants.Queries;
using Giantnodes.Service.Tenants.Abstractions.Views.Tenants;
using Giantnodes.Service.Tenants.Domain.Entities;
using Giantnodes.Service.Tenants.Persistance;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Tenants.Application.Features.Tenants.Queries
{
    public class GetTenantByIdConsumer : IConsumer<GetTenantByIdQuery>
    {
        private readonly ApplicationDbContext _database;
        private readonly IMapper _mapper;

        public GetTenantByIdConsumer(ApplicationDbContext database, IMapper mapper)
        {
            this._database = database;
            this._mapper = mapper;
        }

        public async Task Consume(ConsumeContext<GetTenantByIdQuery> context)
        {
            var tenant = await _database.Tenants.FirstOrDefaultAsync(x => x.Id == context.Message.TenantId, context.CancellationToken);
            if (tenant == null)
            {
                await context.RejectAsync<GetTenantByIdQueryRejected, GetTenantByIdQueryRejection>(GetTenantByIdQueryRejection.NotFound);
                return;
            }

            var view = _mapper.Map<Tenant, TenantView>(tenant);
            await context.RespondAsync<GetTenantByIdQueryResult>(new { Tenant = view });
        }
    }
}
