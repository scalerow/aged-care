using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Giantnodes.Infrastructure.EntityFramework.Converters
{
    public class UpperCaseConverter : ValueConverter<string, string>
    {
        public UpperCaseConverter()
            : base(v => v.ToLowerInvariant(), v => v.ToLowerInvariant())
        {
        }
    }
}
