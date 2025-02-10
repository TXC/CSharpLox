using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CodeCrafters.Interpreter.Extensions;

public static partial class StringExtension
{
    public static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input[0].ToString().ToUpper() + input[1..].ToLower()
        };

    public static string[] SplitCamelCase(this string input)
    {
        return SplitCamelCaseRegex().Split(input);
    }

    public static string[] SplitCamelCaseNumber(this string input)
    {
        return SplitCamelCaseRegexNumber().Split(input);
    }

    /// <summary>
    /// Uses Uri.EscapeDataString() based on recommendations on MSDN
    /// http://blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx
    /// </summary>
    public static string UrlEncode(this string input)
    {
        const int maxLength = 32766;

        ArgumentNullException.ThrowIfNull(input);

        if (input.Length <= maxLength)
        {
            return Uri.EscapeDataString(input);
        }

        var sb = new StringBuilder(input.Length * 2);
        var index = 0;

        while (index < input.Length)
        {
            var length = Math.Min(input.Length - index, maxLength);

            while (CharUnicodeInfo.GetUnicodeCategory(input[index + length - 1]) == UnicodeCategory.Surrogate)
            {
                length--;
            }

            var subString = input.Substring(index, length);

            sb.Append(Uri.EscapeDataString(subString));
            index += subString.Length;
        }

        return sb.ToString();
    }

    public static string? UrlEncode(this string input, Encoding encoding)
    {
        var encoded = HttpUtility.UrlEncode(input, encoding);
        return encoded?.Replace("+", "%20");
    }

    public static string UrlDecode(this string input) => HttpUtility.UrlDecode(input);

    public static string UrlDecode(this string input, Encoding encoding) => HttpUtility.UrlDecode(input, encoding);

    public static string RemoveWhitespace(this string input)
        => ReplaceWhitespace(input, "");

    public static string ReplaceWhitespace(this string input, string replacement)
        => RegexRemoveWhiteSpace().Replace(input, replacement);

    [GeneratedRegex(@"(?<!^)(?=[A-Z])")]
    private static partial Regex SplitCamelCaseRegex();

    [GeneratedRegex(@"(?<!^)(?=[A-Z0-9])")]
    private static partial Regex SplitCamelCaseRegexNumber();

    [GeneratedRegex(@"\s+")]
    private static partial Regex RegexRemoveWhiteSpace();
}
