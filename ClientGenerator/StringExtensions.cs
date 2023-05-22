namespace ClientGenerator;

public static class StringExtensions
{
    public static string ToUpperFirst(this string s)
    {
        return char.ToUpper(s[0]) + $"{s.Substring(1, s.Length - 1)}";
    }
}