using Giantnodes.Service.Tenants.Abstractions.Invitations.Requests;
using Giantnodes.Service.Tenants.Application.Features.Invitations.Requests;
using Giantnodes.Service.Tenants.Persistance;
using Giantnodes.Service.Tenants.Shared.Tests.Bogus;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Giantnodes.Service.Tenants.Application.Tests.Features.Invitations
{
    public class SendTenantInviteConsumerUnitTests
    {
        private readonly ApplicationDbContext _database;
        private readonly ServiceProvider _provider;

        public SendTenantInviteConsumerUnitTests()
        {
            _database = new ApplicationDbContext(new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _provider = new ServiceCollection()
                .AddScoped(_ => _database)
                .AddApplicationTestServices()
                .AddMassTransitTestHarness(options =>
                {
                    options.AddConsumer<SendTenantInviteConsumer>();
                })
                .BuildServiceProvider(true);
        }

        [Fact]
        public async Task Reject_When_Tenant_Not_Found()
        {
            // Arrange
            var command = new SendTenantInviteRequest
            {
                TenantId = Guid.NewGuid()
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<SendTenantInviteRequest>();
            var response = await client.GetResponse<SendTenantInviteRequestRejected>(command);

            // Assert
            Assert.True(await harness.Sent.Any<SendTenantInviteRequestRejected>());
            Assert.Equal(SendTenantInviteRequestRejection.TenantNotFound, response.Message.ErrorCode);
        }

        //[Fact]
        //public async Task Reject_When_User_Not_Found()
        //{
        //    // Arrange
        //    var tenant = new TenantFaker().Generate();
        //    _database.Tenants.Add(tenant);
        //    await _database.SaveChangesAsync();

        //    var command = new SendTenantInviteRequest
        //    {
        //        TenantId = tenant.Id,
        //        UserId = Guid.NewGuid(),
        //    };

        //    var harness = _provider.GetRequiredService<ITestHarness>();
        //    await harness.Start();

        //    // Act
        //    var client = harness.GetRequestClient<SendTenantInviteRequest>();
        //    var response = await client.GetResponse<SendTenantInviteRequestRejected>(command);

        //    // Assert
        //    Assert.True(await harness.Sent.Any<SendTenantInviteRequestRejected>());
        //    Assert.Equal(SendTenantInviteRequestRejection.UserNotFound, response.Message.ErrorCode);
        //}
    }
}
