////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Режимы поиска текста
/// </summary>
public enum FindTextModesEnum
{
    /// <summary>
    /// Равен строке
    /// </summary>
    [Description("Равен строке")]
    Equal,

    /// <summary>
    /// Не равен строке
    /// </summary>
    [Description("Не равен строке")]
    NotEqual,

    /// <summary>
    /// Содержит подстроку в строке
    /// </summary>
    [Description("Содержит подстроку в строке")]
    Contains,

    /// <summary>
    /// Не содержит подстроку в строке
    /// </summary>
    [Description("Не содержит подстроку в строке")]
    NotContains
}