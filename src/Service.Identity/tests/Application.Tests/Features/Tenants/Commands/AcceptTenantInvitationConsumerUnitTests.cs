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
    public class AcceptTenantInvitationConsumerUnitTests
    {
        private readonly ApplicationDbContext _database;
        private readonly ServiceProvider _provider;

        public AcceptTenantInvitationConsumerUnitTests()
        {
            _database = new ApplicationDbContext(new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _provider = new ServiceCollection()
                .AddScoped(_ => _database)
                .AddApplicationTestServices()
                .AddMassTransitTestHarness(options =>
                {
                    options.AddConsumer<AcceptTenantInvitationConsumer>();
                })
                .BuildServiceProvider(true);
        }

        [Fact]
        public async Task Reject_When_Invite_Not_Found()
        {
            // Arrange
            var command = new AcceptTenantInvitationCommand
            {
                Token = Guid.NewGuid()
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<AcceptTenantInvitationCommand>();
            var response = await client.GetResponse<AcceptTenantInvitationCommandRejected>(command);

            // Assert
            Assert.True(await harness.Sent.Any<AcceptTenantInvitationCommandRejected>());
            Assert.Equal(AcceptTenantInvitationCommandRejection.NotFound, response.Message.ErrorCode);
        }

        [Fact]
        public async Task Reject_When_Invite_Expired()
        {
            // Arrange
            var user = new ApplicationUserFaker().Generate();
            _database.Users.Add(user);

            var invite = new UserTenantInviteFaker(user).Generate();
            invite.ExpiresAt = DateTime.UtcNow;
            _database.UserTenantInvites.Add(invite);
            await _database.SaveChangesAsync();

            var command = new AcceptTenantInvitationCommand
            {
                Token = invite.Token
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<AcceptTenantInvitationCommand>();
            var response = await client.GetResponse<AcceptTenantInvitationCommandRejected>(command);

            // Assert
            Assert.True(await harness.Sent.Any<AcceptTenantInvitationCommandRejected>());
            Assert.Equal(AcceptTenantInvitationCommandRejection.Expired, response.Message.ErrorCode);
        }

        [Fact]
        public async Task Respond_Result_When_Accepted()
        {
            // Arrange
            var user = new ApplicationUserFaker().Generate();
            _database.Users.Add(user);

            var invite = new UserTenantInviteFaker(user).Generate();
            _database.UserTenantInvites.Add(invite);
            await _database.SaveChangesAsync();

            var command = new AcceptTenantInvitationCommand
            {
                Token = invite.Token
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<AcceptTenantInvitationCommand>();
            await client.GetResponse<AcceptTenantInvitationCommandResult>(command);

            // Assert
            Assert.True(await harness.Sent.Any<AcceptTenantInvitationCommandResult>());
        }
    }
}
