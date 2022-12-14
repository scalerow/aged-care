using Giantnodes.Service.Identity.Abstractions.Registration.Commands;
using Giantnodes.Service.Identity.Abstractions.Registration.Events;
using Giantnodes.Service.Identity.Application.Features.Registration.Commands;
using Giantnodes.Service.Identity.Persistence;
using Giantnodes.Service.Identity.Shared.Tests.Bogus;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Giantnodes.Service.Identity.Application.Tests.Features.Registration.Commands
{
    public class CreateUserConsumerUnitTests
    {
        private readonly ApplicationDbContext _database;
        private readonly ServiceProvider _provider;

        private readonly string SecurePassword = "-rq25DKt?eYD%L,v";

        public CreateUserConsumerUnitTests()
        {
            _database = new ApplicationDbContext(new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _provider = new ServiceCollection()
                .AddScoped(_ => _database)
                .AddApplicationTestServices()
                .AddMassTransitTestHarness(options =>
                {
                    options.AddConsumer<CreateUserConsumer>();
                })
                .BuildServiceProvider(true);
        }

        [Fact]
        public async Task Reject_When_Duplicate_Email_Exists()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "example@giantnodes.com",
                GivenName = "John",
                FamilyName = "Doe",
                Password = SecurePassword
            };

            var existing = new ApplicationUserFaker().Generate();
            existing.Email = command.Email;
            existing.NormalizedEmail = command.Email.ToUpper();

            _database.Users.Add(existing);
            await _database.SaveChangesAsync();

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<CreateUserCommand>();
            var response = await client.GetResponse<CreateUserCommandRejected>(command);

            // Assert
            Assert.True(await harness.Sent.Any<CreateUserCommandRejected>());
            Assert.Equal(CreateUserCommandRejection.DuplicateEmail, response.Message.ErrorCode);
        }

        [Fact]
        public async Task Reject_When_Weak_Password_Provided()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "example@giantnodes.com",
                GivenName = "John",
                FamilyName = "Doe",
                Password = "password"
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<CreateUserCommand>();
            var response = await client.GetResponse<CreateUserCommandRejected>(command);

            // Assert
            Assert.True(await harness.Sent.Any<CreateUserCommandRejected>());
            Assert.Equal(CreateUserCommandRejection.PasswordTooWeak, response.Message.ErrorCode);
        }

        [Fact]
        public async Task Sends_SendEmailConfirmationCommand_When_User_Registers()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "example@giantnodes.com",
                GivenName = "John",
                FamilyName = "Doe",
                Password = SecurePassword
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<CreateUserCommand>();
            await client.GetResponse<CreateUserCommandResult>(command);

            // Assert
            Assert.True(await harness.Sent.Any<SendUserEmailConfirmationCommand>());
        }

        [Fact]
        public async Task Publish_Event_When_User_Registers()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "example@giantnodes.com",
                GivenName = "John",
                FamilyName = "Doe",
                Password = SecurePassword
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<CreateUserCommand>();
            await client.GetResponse<CreateUserCommandResult>(command);

            // Assert
            Assert.True(await harness.Published.Any<UserCreatedEvent>());
        }
    }
}
