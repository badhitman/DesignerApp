using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SharedLib;

/// <summary>
/// Валидация корректности пространства имён
/// </summary>
public class NamespaceAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is string ns)
        {
            if (CheckNS(ns))
                return true;
            else
                ErrorMessage = "Некорректное пространство имён (namespace)";
        }
        return false;
    }

    static bool CheckNS(string ns)
    {
        if (string.IsNullOrEmpty(ns))
            return false;

        return ns.Split('.').All(x => Regex.IsMatch(x, GlobalStaticConstants.SYSTEM_NAME_TEMPLATE));
    }
}
