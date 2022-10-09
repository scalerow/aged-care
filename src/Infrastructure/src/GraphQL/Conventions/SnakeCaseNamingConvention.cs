using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Giantnodes.Infrastructure.GraphQL
{
    public class SnakeCaseNamingConvention : DefaultNamingConventions
    {
        private readonly string SnakeCaseRegexPattern = @"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+";

        public override NameString GetMemberName(MemberInfo member, MemberKind kind)
        {
            if (kind != MemberKind.ObjectField && kind != MemberKind.InterfaceField && kind != MemberKind.InputObjectField)
                return base.GetMemberName(member, kind);

            var pattern = new Regex(SnakeCaseRegexPattern);
            return string.Join("_", pattern.Matches(member.Name)).ToLower();
        }

        public override NameString GetEnumValueName(object value)
        {
            var input = value.ToString();
            if (input == null)
                return base.GetEnumValueName(value);

            var pattern = new Regex(SnakeCaseRegexPattern);
            return string.Join("_", pattern.Matches(input)).ToUpper();
        }
    }
}
