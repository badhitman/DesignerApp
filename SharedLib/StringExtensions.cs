using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SharedLib;

/// <summary>
/// StringExtensions
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// NullIfEmpty
    /// </summary>
    public static string? NullIfEmpty(this string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

    static readonly string[] splitDelimiters = [" ", "\n", "\r", "\t", ",", ";", "|", @"\n", @"\r", @"\t", "!"];

    /// <summary>
    /// Перечень числовых значений, разделённых пробелами преобразует в массив числел
    /// </summary>
    public static ImmutableList<decimal> SplitToDecimalList(this string data)
    {
        static string _normString(string _raw)
             => _raw
             .Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator)
             .Replace(",", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);

        static decimal parseString(string _valString)
        {
            _valString = _normString(_valString);
            if (decimal.TryParse(_valString, out decimal res))
                return res;

            return 0;
        }

        return [.. Regex.Split(data, @"\s+")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(parseString)];
    }

    /// <summary>
    /// SplitToList
    /// </summary>
    public static ImmutableList<string> SplitToList(this string data)
        => SplitToListInternal(data, splitDelimiters);

    /// <summary>
    /// SplitToListWithoutSpace
    /// </summary>
    public static ImmutableList<string> SplitToListWithoutSpace(this string data)
        => SplitToListInternal(data, splitDelimiters[1..]);

    private static ImmutableList<string> SplitToListInternal(string data, string[] delimiters)
    {
        if (data == null || string.IsNullOrWhiteSpace(data))
            return [];

        return [.. data
            .Split(delimiters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct()
            .Order()];
    }

    /// <summary>
    /// SplitToStringList
    /// </summary>
    public static string SplitToStringList(this string data, char quote = '\'')
    {
        if (data == null)
            return string.Empty;

        return string.Join(", ", data.SplitToList().Select(x => $"{quote}{x}{quote}"));
    }

    /// <summary>
    /// SplitIntoChunks
    /// </summary>
    public static IEnumerable<string> SplitIntoChunks(this string? toSplit, int chunkSize, bool splitLines = false)
    {
        if (toSplit == null)
            yield break;

        if (toSplit.Length <= chunkSize)
        {
            yield return toSplit;
            yield break;
        }

        if (splitLines)
        {
            var sb = new StringBuilder();
            var lines = toSplit.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (sb.Length + lines[i].Length > chunkSize)
                {
                    yield return sb.ToString();
                    sb.Clear();
                }
                sb.Append(lines[i]).Append('\n');
            }

            yield return sb.ToString();
        }
        else
        {
            int stringLength = toSplit.Length;
            int chunksRequired = (int)Math.Ceiling((decimal)stringLength / (decimal)chunkSize);
            int lengthRemaining = stringLength;

            for (int i = 0; i < chunksRequired; i++)
            {
                int lengthToUse = Math.Min(lengthRemaining, chunkSize);
                yield return toSplit.Substring(chunkSize * i, lengthToUse);
                lengthRemaining -= lengthToUse;
            }
        }
    }
}
