////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Режимы выбора рубрики
/// </summary>
public enum ModesSelectRubricsEnum
{
    /// <summary>
    /// Разрешить без рубрик
    /// </summary>
    [Description("Разрешить без рубрик")]
    AllowWithoutRubric = 10,

    /// <summary>
    /// Выбор корневого, а вложенные по желанию
    /// </summary>
    [Description("Выбор корневого, а вложенные по желанию")]
    SelectRootAndFree = 20,

    /// <summary>
    /// Строго заполнить всю иерархию
    /// </summary>
    [Description("Строго заполнить всю иерархию")]
    Strict = 30
}