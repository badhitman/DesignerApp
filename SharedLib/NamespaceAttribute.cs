using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SharedLib;

/// <summary>
/// Валидация корректности пространства имён
/// </summary>
public partial class NamespaceAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is string ns && CheckNS(ns))
            return true;

        ErrorMessage = "Некорректное пространство имён (namespace)";
        return false;
    }

    static bool CheckNS(string ns)
    {
        if (string.IsNullOrEmpty(ns))
            return false;

        return ns.Split('.').All(x => MyRegexSystemName().IsMatch(x));
    }

    [GeneratedRegex(GlobalStaticConstants.SYSTEM_NAME_TEMPLATE, RegexOptions.Compiled)]
    private static partial Regex MyRegexSystemName();
}