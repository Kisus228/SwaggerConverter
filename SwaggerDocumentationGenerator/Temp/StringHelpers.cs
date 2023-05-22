using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;

namespace SwaggerDocumentationGenerator.Temp;

public static class StringHelpers
	{
		public static string Require(this string some)
		{
			if (string.IsNullOrEmpty(some))
				throw new InvalidOperationException("required settings are missing");
			return some;
		}

		public static string Coalesce(params string[] strings)
		{
			return strings.FirstOrDefault(s => !string.IsNullOrEmpty(s));
		}

		public static string Plural(decimal number, string form1, string form2)
		{
			return Plural(number, form1, form2, form2);
		}

		public static string Plural(decimal number, string form1, string form2, string form5)
		{
			number = Math.Abs(number);
			return number % 10 == 1 && number % 100 != 11
					   ? form1
					   : (number % 10 >= 2 && number % 10 <= 4 && (number % 100 < 10 || number % 100 >= 20) ? form2 : form5);
		}

		public static string AccusativePlural(decimal number, string form1, string form2)
		{
			number = Math.Abs(number);
			if (number%10 == 1 && number%100 != 11)
				return form1;
			if (number%10 >= 2 && number%10 <= 4 && (number%100 < 10 || number%100 >= 20))
				return form1;
			return form2;
		}

		public static IEnumerable<string> SplitByLength(this string s, int maxLength)
		{
			var rest = s.EmptyIfNull();
			string item;
			while (!string.IsNullOrEmpty(item = rest.SafeSubstring(0, maxLength)))
			{
				yield return item;
				rest = rest.SafeSubstring(item.Length);
			}
		}

		public static string SafeSubstring(this string s, int startIndex, int length = -1)
		{
			if (startIndex >= s.Length)
				return string.Empty;

			return length == -1 ? s.Substring(startIndex) : s.Substring(startIndex, Math.Min(s.Length - startIndex, length));
		}

		public static string JoinWithRussianStyle(IEnumerable<string> list)
		{
			return new Regex(", ([^,]*)$").Replace(string.Join(", ", list), " и $1");
		}

		public static string ExpandMacros(string source, NameValueCollection keywords, string macroToken = "%")
		{
			if (source == null)
				return null;
			return keywords.Cast<string>()
						   .Aggregate(source, (current, key) => current.Replace(macroToken + key + macroToken, keywords[key]));
		}

		public static string Translate(this string source, string replace)
		{
			if ((replace.Length%2) != 0)
				throw new InvalidOperationException("replace should contain event number of characters");
			var chars = replace.ToCharArray();
			var result = source;
			for (var i = 0; i < chars.Length; i += 2)
				result = result.Replace(chars[i], chars[i + 1]);
			return result;
		}

		private static readonly NameValueCollection russianEnglishLetters = new NameValueCollection
																			{
																				{ "с", "c" },
																				{ "а", "a" },
																				{ "е", "e" },
																				{ "о", "o" },
																				{ "р", "p" },
																				{ "у", "y" },
																				{ "х", "x" }
																			};

		public static string FixRussianLetters(this string source)
		{
			return russianEnglishLetters
				.Cast<string>()
				.Aggregate(source, (current, key) => current
														 .Replace(key, russianEnglishLetters[key])
														 .Replace(key.ToUpper(), russianEnglishLetters[key].ToUpper()));
		}

		public static string ToUnderscore(this string s)
		{
			var resultChars = new List<char>();
			foreach (var c in s)
				if (char.IsUpper(c))
				{
					if (resultChars.Count > 0)
						resultChars.Add('_');
					resultChars.Add(char.ToLower(c));
				}
				else
					resultChars.Add(c);
			return new string(resultChars.ToArray());
		}

		public static string RemoveCRLF(this string s)
		{
			return s?.CollapseCRLF().Replace("\n", "");
		}

		public static string SpacesToNonBreakingSpaces(this string input)
		{
			return input.Replace(' ', nbsp);
		}

		public static string NonBreakingSpacesToSpaces(this string input)
		{
			return input?.Replace(nbsp, ' ');
		}

		public static string CollapseCRLF(this string s)
		{
			if (s == null)
				return null;

			var result = s.Replace('\r', '\n');
			do
			{
				s = result;
				result = s.Replace("\n\n", "\n");
			} while (result != s);
			return result;
		}

		public static string RussianQuotesToEnglishQuotes(this string source)
		{
			return source?.Replace('«', '"').Replace('»', '"');
		}

		public static int IndexOfNth(this string s, string value, int n, int startIndex = 0)
		{
			if (n <= 0)
				throw new ArgumentException();
			if (n == 1)
				return s.IndexOf(value, startIndex);
			var position = s.IndexOf(value, startIndex);
			if (position == -1)
				return -1;
			return IndexOfNth(s, value, n - 1, position + value.Length);
		}

		private static readonly Regex htmlEscapeRegex = new Regex(@"\<(?:\w|/|\!)", RegexOptions.Compiled | RegexOptions.Singleline);
		private static readonly Regex angularEscapeRegex = new Regex("{(?={)", RegexOptions.Compiled);

		public static string EscapeHtmlAndAngularTemplates(this string source)
		{
			if (source == null)
				return null;

			var result = source.DecodeHtmlContinuously();
			result = htmlEscapeRegex.Replace(result, match => $"<{zws}{match.Value.Substring(1)}");
			return angularEscapeRegex.Replace(result, $"{{{zws}");
		}

		public static string DecodeHtmlContinuously(this string source)
		{
			if (string.IsNullOrEmpty(source))
				return source;

			int originalLength;
			do
			{
				originalLength = source.Length;
				source = HttpUtility.HtmlDecode(source);
			} while (source.Length != originalLength);

			return source;
		}

		public const char nbsp = (char) 0x00A0;
		public const char zws = (char) 0x200B;
		public const char mdash = (char) 0x2014;
		public const char bom = (char) 65279;
		public const char thinsp = (char)0x2009;
		public const char minus = (char)0x2212;

		public static string Truncate(this string s, int length, bool withDots = true)
		{
			return s.Length < length
					   ? s :
					withDots
						? s.Substring(0, length - 3) + "..."
						  : s.Substring(0, length);
		}

		public static string ToLower(string value)
		{
			return value.EmptyIfNull().ToLowerInvariant().Trim();
		}

		public static string ToLowerWithoutYo(string value)
		{
			return value.EmptyIfNull().ToLowerInvariant().RemoveLowerYo();
		}

		public static string SquareToAngleBrackets(this string s)
		{
			return s.Replace('[', '<').Replace(']', '>');
		}

		public static string NumberGenitive(int number)
		{
			switch (number)
			{
				case 1:
					return "одного";
				case 2:
					return "двух";
				case 3:
					return "трех";
				case 4:
					return "четырех";
				case 5:
					return "пяти";
				case 6:
					return "шести";
				case 7:
					return "семи";
				case 8:
					return "восьми";
				case 9:
					return "девяти";
				case 10:
					return "десяти";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}