using System.Text.RegularExpressions;

namespace Utilities.Extensions;

public static class StringExtensions
{
    private static Regex CamelCaseStringPattern = new Regex(@"(?<=[a-z])([A-Z])");

    public static bool Contains(this string source, string toCheck, StringComparison comp)
    {
        return source?.IndexOf(toCheck, comp) >= 0;
    }

    public static string ToWords(this string camelCaseString)
    {
        return CamelCaseStringPattern.Replace(camelCaseString, " $1");
    }

    public static string Truncate(this string message, int length, string suffix = null)
    {
        if (message.Length <= length)
        {
            return message;
        }

        var suffixLength = suffix?.Length ?? 0;
        var effectiveLength = message.Length > length ? length : message.Length;

        if (effectiveLength - suffixLength <= 0)
        {
            return message.Substring(0, effectiveLength);
        }

        return message.Substring(0, effectiveLength - suffixLength) + suffix;
    }
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var startUnderscores = Regex.Match(input, @"^_+");
        return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }
}
