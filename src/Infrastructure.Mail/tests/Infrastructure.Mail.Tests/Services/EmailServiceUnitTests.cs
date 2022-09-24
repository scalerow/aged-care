using Giantnodes.Infrastructure.Mail.Abstractions;
using Giantnodes.Infrastructure.Mail.Tests.Bogus;
using Giantnodes.Infrastructure.Mail.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using System.Linq.Expressions;
using System.Net;
using Xunit;

namespace Giantnodes.Infrastructure.Mail.Tests.Services
{
    public class EmailServiceUnitTests
    {
        private readonly ServiceProvider _provider;
        private readonly EmailTemplateFake _template;
        private readonly EmailServiceMock _service;

        public EmailServiceUnitTests()
        {
            _provider = new ServiceCollection()
                .AddMailTestsServices()
                .BuildServiceProvider(true);

            _template = new EmailTemplateFake();
            _service = new EmailServiceMock(_provider.GetRequiredService<IOptions<EmailSettings>>());
        }

        [Fact]
        public async Task Sends_Email_To_Correct_Recipient()
        {
            // Arrange
            var address = new MailboxAddressFaker().Generate();

            // Act
            await _service.SendEmailAsync(_template, address, CancellationToken.None);

            // Assert
            Expression<Func<MimeMessage, bool>> isCorrectEmail = x =>
                x.To.First() == address &&
                x.To.Count == 1;

            _service.Client.Verify(x => x.SendAsync(It.Is(isCorrectEmail), It.IsAny<CancellationToken>(), null), Times.Once);
        }

        [Fact]
        public async Task Sends_Multiple_Emails_To_Correct_Recipients()
        {
            // Arrange
            var addresses = new MailboxAddressFaker().Generate(8);

            // Act
            await _service.SendEmailAsync(_template, addresses, CancellationToken.None);

            // Assert
            Expression<Func<MimeMessage, bool>> isCorrectEmail = x =>
                addresses.Contains(x.To.First()) &&
                x.To.Count == 1;

            _service.Client.Verify(x => x.SendAsync(It.Is(isCorrectEmail), It.IsAny<CancellationToken>(), null), Times.Exactly(addresses.Count));
        }

        [Fact]
        public async Task Filters_Out_Duplicate_Recipients()
        {
            // Arrange
            var unique = new MailboxAddressFaker().Generate(8);
            var addresses = new List<MailboxAddress>(unique);
            addresses.AddRange(unique.Take(4));

            // Act
            await _service.SendEmailAsync(_template, addresses, CancellationToken.None);

            // Assert
            Expression<Func<MimeMessage, bool>> isCorrectEmail = x =>
                addresses.Contains(x.To.First()) &&
                x.To.Count == 1;

            _service.Client.Verify(x => x.SendAsync(It.Is(isCorrectEmail), It.IsAny<CancellationToken>(), null), Times.Exactly(unique.Count));
        }

        [Fact]
        public async Task Disconnects_SmtpClient_When_Complete()
        {
            // Act
            await _service.SendEmailAsync(_template, new MailboxAddressFaker().Generate(), CancellationToken.None);

            // Assert
            _service.Client.Verify(x => x.DisconnectAsync(true, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Disconnects_SmtpClient_When_Exception_Thrown()
        {
            // Arrange
            _service.Client.Setup(x => x.AuthenticateAsync(It.IsAny<ICredentials>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            // Act
            try
            {
                await _service.SendEmailAsync(_template, new MailboxAddressFaker().Generate(), CancellationToken.None);
            }
            catch (Exception)
            {
            }
            finally
            {
                // Assert
                _service.Client.Verify(x => x.DisconnectAsync(true, It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
