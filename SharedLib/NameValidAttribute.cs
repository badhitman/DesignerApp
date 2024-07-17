using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SharedLib;

/// <summary>
/// Валидация корректности имени
/// </summary>
public partial class NameValidAttribute : ValidationAttribute
{
    /// <summary>
    /// Разрешить пустую строку
    /// </summary>
    public bool AllowEmptyStrings { get; set; }

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is string n)
        {
            if (AllowEmptyStrings && n == "")
                return true;
            if (MyRegexName().IsMatch(n) && MyRegexPrefixCheck().IsMatch(n) && MyRegexPostfixCheck().IsMatch(n))
                return true;
        }
        ErrorMessage = "Некорректное имя: первым и последним символом должна идти буква";
        return false;
    }

    [GeneratedRegex(@"^\w+.*\w$", RegexOptions.Compiled)]
    private static partial Regex MyRegexName();
    [GeneratedRegex(@"^[^\d_]", RegexOptions.Compiled)]
    private static partial Regex MyRegexPrefixCheck();
    [GeneratedRegex(@"[^\d_]$")]
    private static partial Regex MyRegexPostfixCheck();
}