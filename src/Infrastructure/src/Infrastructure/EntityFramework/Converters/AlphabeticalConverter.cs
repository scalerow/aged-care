using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.RegularExpressions;

namespace Giantnodes.Infrastructure.EntityFramework.Converters
{
    public class AlphabeticalConverter : ValueConverter<string, string>
    {
        private static readonly string AlphabeticalRegexPattern = @"[^a-zA-Z ]";

        public AlphabeticalConverter()
            : base(
                  v => Regex.Replace(v, AlphabeticalRegexPattern, string.Empty),
                  v => Regex.Replace(v, AlphabeticalRegexPattern, string.Empty))
        {
        }
    }
}
