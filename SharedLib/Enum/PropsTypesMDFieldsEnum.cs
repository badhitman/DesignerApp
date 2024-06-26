////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Типы параметров 'опций расширения'
/// </summary>
public enum PropsTypesMDFieldsEnum
{
    /// <summary>
    /// None
    /// </summary>
    [Description("Нет")]
    None,

    /// <summary>
    /// Маска
    /// </summary>
    [Description("Маска")]
    TextMask,

    /// <summary>
    /// Шаблон
    /// </summary>
    [Description("Шаблон")]
    Template
}