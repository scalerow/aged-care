using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Giantnodes.Infrastructure.EntityFramework.Converters
{
    public class LowerCaseConverter : ValueConverter<string, string>
    {
        public LowerCaseConverter()
            : base(v => v.ToLowerInvariant(), v => v.ToLowerInvariant())
        {
        }
    }
}
