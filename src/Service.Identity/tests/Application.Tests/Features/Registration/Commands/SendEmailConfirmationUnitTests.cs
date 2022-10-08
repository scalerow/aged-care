using Bogus;
using Giantnodes.Infrastructure.Mail.Services;
using Giantnodes.Service.Identity.Abstractions.Registration.Commands;
using Giantnodes.Service.Identity.Application.Features.Registration.Commands;
using Giantnodes.Service.Identity.Domain.Identity;
using Giantnodes.Service.Identity.Mail.Templates;
using Giantnodes.Service.Identity.Persistence;
using Giantnodes.Service.Identity.Shared.Tests.Bogus;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Giantnodes.Service.Identity.Application.Tests.Features.Registration.Commands
{
    public class SendEmailConfirmationUnitTests
    {
        private readonly ApplicationDbContext _database;
        private readonly ServiceProvider _provider;

        public SendEmailConfirmationUnitTests()
        {
            _database = new ApplicationDbContext(new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _provider = new ServiceCollection()
                .AddScoped(_ => _database)
                .AddApplicationTestServices()
                .AddMassTransitTestHarness(options =>
                {
                    options.AddConsumer<SendUserEmailConfirmationConsumer>()
                        .Endpoint(e => e.Name = KebabCaseEndpointNameFormatter.Instance.Message<SendUserEmailConfirmationCommand>());
                })
                .BuildServiceProvider(true);
        }

        [Fact]
        public async Task Reject_When_User_Not_Found()
        {
            // Arrange
            var command = new SendUserEmailConfirmationCommand
            {
                Email = new Faker().Internet.Email()
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<SendUserEmailConfirmationCommand>();
            var response = await client.GetResponse<SendUserEmailConfirmationCommandRejected>(command);

            // Assert
            Assert.True(await harness.Sent.Any<SendUserEmailConfirmationCommandRejected>());
            Assert.Equal(SendUserEmailConfirmationCommandRejection.NotFound, response.Message.ErrorCode);
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

            var command = new SendUserEmailConfirmationCommand
            {
                Email = user.Email
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var client = harness.GetRequestClient<SendUserEmailConfirmationCommand>();
            var response = await client.GetResponse<SendUserEmailConfirmationCommandRejected>(command);

            // Assert
            Assert.True(await harness.Sent.Any<SendUserEmailConfirmationCommandRejected>());
            Assert.Equal(SendUserEmailConfirmationCommandRejection.AlreadyConfirmed, response.Message.ErrorCode);
        }

        [Fact]
        public async Task Send_Email_With_Confirmation_Code()
        {
            // Arrange
            using var scope = _provider.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = new ApplicationUserFaker().Generate();
            user.EmailConfirmed = false;
            await manager.CreateAsync(user);

            var command = new SendUserEmailConfirmationCommand
            {
                Email = user.Email
            };

            var harness = _provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            // Act
            var topic = KebabCaseEndpointNameFormatter.Instance.Message<SendUserEmailConfirmationCommand>();
            var endpoint = await harness.Bus.GetSendEndpoint(new Uri($"queue:{topic}"));
            await endpoint.Send<SendUserEmailConfirmationCommand>(command);

            // Assert
            Assert.True(await harness.Consumed.Any<SendUserEmailConfirmationCommand>());

            Expression<Func<MailboxAddress, bool>> isCorrectAddress = x =>
                x.Name == user.FullName &&
                x.Address == user.Email;

            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            Mock.Get(emailService).Verify(x => x.SendEmailAsync(It.IsAny<EmailConfirmationTemplate>(), It.Is(isCorrectAddress), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
