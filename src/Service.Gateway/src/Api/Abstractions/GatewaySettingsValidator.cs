using Microsoft.Extensions.Options;

namespace Giantnodes.Service.Gateway.Api.Abstractions
{
    public class GatewaySettingsValidator : IValidateOptions<GatewaySettings>
    {
        public ValidateOptionsResult Validate(string name, GatewaySettings? options)
        {
            if (options == null)
                return ValidateOptionsResult.Fail($"The '{nameof(GatewaySettings)}' configuration object is null.");

            if (string.IsNullOrWhiteSpace(options.Name))
                return ValidateOptionsResult.Fail($"Property '{nameof(options.Name)}' cannot be blank.");

            if (options.Domains == null)
                return ValidateOptionsResult.Fail($"The '{nameof(options.Domains)}' configuration object is null.");

            if (options.Domains.Count == 0)
                return ValidateOptionsResult.Fail($"The '{nameof(options.Domains)}' array has no elements.");

            foreach (var domain in options.Domains)
            {
                if (string.IsNullOrWhiteSpace(domain.Name))
                    return ValidateOptionsResult.Fail($"Property '{nameof(domain.Name)}' cannot be blank.");

                if (string.IsNullOrWhiteSpace(domain.SchemaConnection))
                    return ValidateOptionsResult.Fail($"Property '{nameof(domain.SchemaConnection)}' cannot be blank.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
