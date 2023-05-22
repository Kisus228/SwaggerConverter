using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SwaggerDocumentationGenerator.Temp;

public static class StringExtensions
{
	private const string quote = @"""";

	public static string Nbsp = "\u00A0";
	private const string ThinSP = "\x2009";

	public static string HtmlEncode(this string s)
	{
		return WebUtility.HtmlEncode(s);
	}

	public static string Quote(this string src)
	{
		return quote + src.Replace(@"\", @"\\").Replace(quote, @"\""") + quote;
	}

	public static string TextQuote(this string src)
	{
		return string.Format("«{0}»", src);
	}

	public static string ReverseString(this string s)
	{
		return new string(s.AsEnumerable().Reverse().ToArray());
	}

	public static bool ContainsIgnoreCase(this string value, string sample)
	{
		return value.Contains(sample, StringComparison.OrdinalIgnoreCase);
	}

	public static bool Contains(this string value, string sample, StringComparison comparison)
	{
		if (string.IsNullOrEmpty(sample))
			return true;
		if (string.IsNullOrEmpty(value))
			return false;
		return value.IndexOf(sample, comparison) >= 0;
	}

	public static int Count(this string value, params char[] symbols)
	{
		var result = 0;
		var index = -1;
		while ((index = value.IndexOfAny(symbols, index + 1)) >= 0)
			result++;
		return result;
	}

	public static IEnumerable<string> Path(this string s)
	{
		return s.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
	}

	public static string? NullIfEmpty(this string? s)
	{
		return string.IsNullOrEmpty(s) ? null : s;
	}

	public static string? NullIfWhitespace(this string? s)
	{
		return string.IsNullOrWhiteSpace(s) ? null : s;
	}

	public static string EmptyIfNull(this string? s)
	{
		return s ?? string.Empty;
	}

	public static bool IsFilled(this string? s)
	{
		return !string.IsNullOrEmpty(s);
	}

	public static bool IsNullOrWhitespace(this string? s)
	{
		return string.IsNullOrWhiteSpace(s);
	}

	public static string Capitalize(this string s)
	{
		if (string.IsNullOrEmpty(s) || char.IsUpper(s[0]))
			return s;
		var charArray = s.ToCharArray();
		charArray[0] = char.ToUpper(charArray[0], Culture.Russian);
		return new string(charArray);
	}

	public static string Decapitalize(this string s)
	{
		if (string.IsNullOrEmpty(s) || char.IsLower(s[0]))
			return s;
		var charArray = s.ToCharArray();
		charArray[0] = char.ToLower(charArray[0], Culture.Russian);
		return new string(charArray);
	}

	public static bool HasMeaningfulChars(this string s)
	{
		return s != null && s.Any(c => !char.IsWhiteSpace(c));
	}

	public static string Padl(this string s, int totalWidth, char paddingChar)
	{
		return s == null
			? new string(paddingChar, totalWidth)
			: s.PadLeft(totalWidth, paddingChar);
	}

	public static string[] Split(this string s,
	                             string separator,
	                             StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
	{
		return s.Split(new[] { separator }, options);
	}

	public static string[] SplitByChars(this string s,
	                                    char[] separators,
	                                    StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
	{
		return s.Split(separators, options);
	}

	public static string RemoveWhitespaces(this IEnumerable<char> s)
	{
		return new string(s.Where(c => !char.IsWhiteSpace(c) && c != StringHelpers.zws && c != StringHelpers.bom && c != StringHelpers.thinsp).ToArray());
	}

	public static string RemoveChar(this string s, char ch)
	{
		//возможно, можно как-то быстрее удалять заданный символ из строки
		return s.Replace(new string(ch, 1), string.Empty);
	}

	public static string RemoveZws(this IEnumerable<char> s)
	{
		return new string(s.Where(c => c != StringHelpers.zws).ToArray());
	}

	public static byte[] ToByteArray(this string s, Encoding encoding = null)
	{
		return (encoding ?? Encoding.UTF8).GetBytes(s);
	}

	public static string IfWhitespace(this string s, string defaultValue)
	{
		return s.NullIfWhitespace() ?? defaultValue;
	}

	public static IEnumerable<string> SplitCaseInsensitive(this string s, string delimiter)
	{
		int delimiterIndex, resultIndex = 0;
		while ((delimiterIndex = s.IndexOf(delimiter, resultIndex, StringComparison.OrdinalIgnoreCase)) >= 0)
		{
			yield return s.Substring(resultIndex, delimiterIndex - resultIndex);
			resultIndex = delimiterIndex + delimiter.Length;
		}

		yield return s.Substring(resultIndex);
	}

	public static bool EqualsIgnoringCase(this string s1, string s2)
	{
		return string.Equals(s1, s2, StringComparison.InvariantCultureIgnoreCase);
	}

	public static string RemoveYo(this string str)
	{
		return RemoveLowerYo(str).Replace("Ё", "Е");
	}

	public static string RemoveLowerYo(this string str)
	{
		return str.EmptyIfNull().Replace("ё", "е");
	}

	public static string RemoveUnicodeLineTerminators(this string value)
	{
		return value == null ? null : value.Replace("\u2028", "").Replace("\u2029", "");
	}

	public static string WithPrefix(this string s, string prefix)
	{
		return s.StartsWith(prefix) ? s : prefix + s;
	}

	public static string ExcludePrefix(this string s, string prefix, bool ignoreCase = false)
	{
		return s.StartsWith(prefix, ignoreCase, null) ? s.Substring(prefix.Length) : s;
	}

	public static string ExcludeSuffix(this string s, string suffix)
	{
		return s.EndsWith(suffix) ? s.Substring(0, s.Length - suffix.Length) : s;
	}

	public static string ExcludeSuffixes(this string s, string[] suffixes)
	{
		var result = s;
		foreach (var suffix in suffixes)
			result = result.ExcludeSuffix(suffix);
		return result;
	}

	public static string WithSuffix(this string s, string suffix)
	{
		return s.EndsWith(suffix) ? s : s + suffix;
	}

	public static string ReplaceBackslash(this string s)
	{
		return s.Replace('\\', '/');
	}

	public static void Append(this StringBuilder builder, params string[] args)
	{
		foreach (var s in args)
			builder.Append(s);
	}

	public static string LimitLength(this string s, int maxLength)
	{
		return string.IsNullOrEmpty(s) || s.Length < maxLength ? s : s.Substring(0, maxLength);
	}

	private static readonly Regex multipleSpacesRegex = new Regex("[ ]{2,}", RegexOptions.Compiled);

	public static string CollapseMultipleSpaces(this string s)
	{
		return string.IsNullOrEmpty(s) ? s : multipleSpacesRegex.Replace(s, " ");
	}

	public static bool IsAllZeros(this string? s)
	{
		return !s.IsNullOrWhitespace() && s!.All(c => c == '0');
	}

	public static string WithZerosPrefix(this string s, int zerosAmount)
	{
		return s.IsFilled() ? new string('0', zerosAmount) + s : s;
	}

	public static string Replace(this string source, char[] charactersForReplace, string replaceWith) =>
		source.Split(charactersForReplace).JoinStrings(replaceWith);

	public static string ToCamelCase(this string value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof (value));
		if (value.Length > 0 && char.IsUpper(value[0]))
			value = char.ToLower(value[0]).ToString() + value.Substring(1);
		return value;
	}
}