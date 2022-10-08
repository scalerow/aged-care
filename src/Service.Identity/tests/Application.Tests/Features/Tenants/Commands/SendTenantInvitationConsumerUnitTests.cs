using Giantnodes.Service.Identity.Abstractions.Tenants.Commands;
using Giantnodes.Service.Identity.Application.Features.Tenants.Commands;
using Giantnodes.Service.Identity.Persistence;
using Giantnodes.Service.Identity.Shared.Tests.Bogus;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Giantnodes.Service.Identity.Application.Tests.Features.Tenants.Commands
{
    public class SendTenantInvitationConsumerUnitTests
    {
        private readonly ApplicationDbContext _database;
        private readonly ServiceProvider _provider;

        public SendTenantInvitationConsumerUnitTests()
        {
            _database = new ApplicationDbContext(new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _provider = new ServiceCollection()
                .AddScoped(_ => _database)
                .AddApplicationTestServices()
                .AddMassTransitTestHarness(options =>
                {
                    options.AddConsumer<SendTenantInvitationConsumer>();
                })
                .BuildServiceProvider(true);
        }

        [Fact]
        public async Task Reject_When_User_Not_Found()
        {
            // Arrange
            var command = new SendTenantInvitationCommand
            {
                UserId = Guid.NewGuid()
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<SendTenantInvitationCommand>();
            var response = await client.GetResponse<SendTenantInvitationCommandRejected>(command);

            // Assert
            Assert.True(await harness.Sent.Any<SendTenantInvitationCommandRejected>());
            Assert.Equal(SendTenantInvitationCommandRejection.UserNotFound, response.Message.ErrorCode);
        }

        [Fact]
        public async Task Reject_When_User_Already_Present_In_Tenant()
        {
            // Arrange
            var user = new ApplicationUserFaker().Generate();
            _database.Users.Add(user);

            var connection = new UserTenantFaker(user).Generate();
            _database.UserTenants.Add(connection);

            await _database.SaveChangesAsync();

            var command = new SendTenantInvitationCommand
            {
                UserId = user.Id,
                TenantId = connection.TenantId,
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<SendTenantInvitationCommand>();
            var response = await client.GetResponse<SendTenantInvitationCommandRejected>(command);

            // Assert
            Assert.True(await harness.Sent.Any<SendTenantInvitationCommandRejected>());
            Assert.Equal(SendTenantInvitationCommandRejection.UserAlreadyPresent, response.Message.ErrorCode);
        }
    }
}
