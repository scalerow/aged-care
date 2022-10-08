using Bogus;
using Giantnodes.Service.Identity.Abstractions.Registration.Events;
using Giantnodes.Service.Identity.Abstractions.Registration.Requests;
using Giantnodes.Service.Identity.Application.Features.Registration.Requests;
using Giantnodes.Service.Identity.Domain.Identity;
using Giantnodes.Service.Identity.Persistence;
using Giantnodes.Service.Identity.Shared.Tests.Bogus;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Giantnodes.Service.Identity.Application.Tests.Features.Registration.Requests
{
    public class ConfirmEmailConsumerUnitTests
    {
        private readonly ApplicationDbContext _database;
        private readonly ServiceProvider _provider;

        public ConfirmEmailConsumerUnitTests()
        {
            _database = new ApplicationDbContext(new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _provider = new ServiceCollection()
                .AddScoped(_ => _database)
                .AddApplicationTestServices()
                .AddMassTransitTestHarness(options =>
                {
                    options.AddConsumer<ConfirmEmailConsumer>();

                })
                .BuildServiceProvider(true);
        }

        [Fact]
        public async Task Reject_When_User_Not_Found()
        {
            // Arrange
            var command = new ConfirmEmailRequest
            {
                Email = "exmaple@giantnodes.com",
                Token = new Faker().Random.Word()
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<ConfirmEmailRequest>();
            var response = await client.GetResponse<ConfirmEmailRequestRejected>(command);

            // Assert
            Assert.True(await harness.Sent.Any<ConfirmEmailRequestRejected>());
            Assert.Equal(ConfirmEmailRequestRejection.NotFound, response.Message.ErrorCode);
        }

        [Fact]
        public async Task Reject_When_Email_Already_Confirmed()
        {
            // Arrange
            using var scope = _provider.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = new ApplicationUserFaker().Generate();
            user.EmailConfirmed = true;
            await manager.CreateAsync(user);

            var command = new ConfirmEmailRequest
            {
                Email = user.Email,
                Token = new Faker().Random.Word()
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<ConfirmEmailRequest>();
            var response = await client.GetResponse<ConfirmEmailRequestRejected>(command);

            // Assert
            Assert.True(await harness.Sent.Any<ConfirmEmailRequestRejected>());
            Assert.Equal(ConfirmEmailRequestRejection.AlreadyConfirmed, response.Message.ErrorCode);
        }

        [Fact]
        public async Task Reject_When_Verificaion_Code_Invalid()
        {
            // Arrange
            using var scope = _provider.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = new ApplicationUserFaker().Generate();
            user.EmailConfirmed = false;
            await manager.CreateAsync(user);

            var command = new ConfirmEmailRequest
            {
                Email = user.Email,
                Token = new Faker().Random.Word()
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<ConfirmEmailRequest>();
            var response = await client.GetResponse<ConfirmEmailRequestRejected>(command);

            // Assert
            Assert.True(await harness.Sent.Any<ConfirmEmailRequestRejected>());
            Assert.Equal(ConfirmEmailRequestRejection.IdentityError, response.Message.ErrorCode);
        }

        [Fact]
        public async Task Publish_UserConfirmedEmailEvent_When_Email_Confirmed()
        {
            // Arrange
            using var scope = _provider.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = new ApplicationUserFaker().Generate();
            user.EmailConfirmed = false;
            await manager.CreateAsync(user);

            var code = await manager.GenerateEmailConfirmationTokenAsync(user);
            var command = new ConfirmEmailRequest
            {
                Email = user.Email,
                Token = code
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<ConfirmEmailRequest>();
            await client.GetResponse<ConfirmEmailRequestResult>(command);

            // Assert
            Assert.True(await harness.Published.Any<UserConfirmedEmailEvent>());
        }

        [Fact]
        public async Task Responds_ConfirmEmailRequestResult_When_Email_Confirmed()
        {
            // Arrange
            using var scope = _provider.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = new ApplicationUserFaker().Generate();
            user.EmailConfirmed = false;
            await manager.CreateAsync(user);

            var code = await manager.GenerateEmailConfirmationTokenAsync(user);
            var command = new ConfirmEmailRequest
            {
                Email = user.Email,
                Token = code
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<ConfirmEmailRequest>();
            await client.GetResponse<ConfirmEmailRequestResult>(command);

            // Assert
            Assert.True(await harness.Sent.Any<ConfirmEmailRequestResult>());
        }
    }
}
