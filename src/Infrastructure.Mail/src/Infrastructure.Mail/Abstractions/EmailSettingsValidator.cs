using Microsoft.Extensions.Options;

namespace Giantnodes.Infrastructure.Mail.Abstractions
{
    public class EmailSettingsValidator : IValidateOptions<EmailSettings>
    {
        public ValidateOptionsResult Validate(string name, EmailSettings? options)
        {
            if (options is null)
                return ValidateOptionsResult.Fail($"The '{nameof(EmailSettings)}' Configuration object is null.");

            if (string.IsNullOrWhiteSpace(options.Host))
                return ValidateOptionsResult.Fail($"Property '{nameof(options.Host)}' cannot be blank.");

            if (options.Port == 0)
                return ValidateOptionsResult.Fail($"Property '{nameof(options.Port)}' cannot be blank.");

            if (string.IsNullOrWhiteSpace(options.Username))
                return ValidateOptionsResult.Fail($"Property '{nameof(options.Username)}' cannot be blank.");

            if (string.IsNullOrWhiteSpace(options.Password))
                return ValidateOptionsResult.Fail($"Property '{nameof(options.Password)}' cannot be blank.");

            return ValidateOptionsResult.Success;
        }
    }
}
