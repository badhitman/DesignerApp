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
        ErrorMessage = "Некорректное имя: первым и последним символом должна идти буква";

        if (value is null)
            return AllowEmptyStrings;

        if (value is string n)
        {
            if (AllowEmptyStrings && n == "")
                return true;

            return MyRegexName().IsMatch(n) &&
                MyRegexPrefixCheck().IsMatch(n) &&
                MyRegexPostfixCheck().IsMatch(n);
        }

        return false;
    }

    /// <summary>
    /// Начинается с символа и заканчивается символом.
    /// </summary>
    /// <remarks>Символом считается так же цифры и знак нижнего подчёркивания:
    /// требуется доп проверка для исключения цифр и знак подчёркивания в начале (см: <see cref="MyRegexPrefixCheck"/>), а так же следует исключить возможность окончание на знак подчёркивания (см: <see cref="MyRegexPostfixCheck"/>)</remarks>
    [GeneratedRegex(@"^\w+.*\w$", RegexOptions.Compiled)]
    private static partial Regex MyRegexName();

    /// <summary>
    /// В начале не цифра и не знак подчёркивания.
    /// </summary>
    /// <remarks>Т.е. в сочетании с <see cref="MyRegexName"/> достигается утверждение: начало имени может быть только буква!</remarks>
    [GeneratedRegex(@"^[^\d_]", RegexOptions.Compiled)]
    private static partial Regex MyRegexPrefixCheck();

    /// <summary>
    /// Окончание не должно быть символом нижнего подчёркивания.
    /// </summary>
    /// <remarks>Т.е. в сочетании с <see cref="MyRegexName"/> достигается утверждение: окончание имени может быть либо буква либо цифра!</remarks>
    [GeneratedRegex(@"[^_]$")]
    private static partial Regex MyRegexPostfixCheck();
}