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
    [Description("Рубрика не обязательна")]
    AllowWithoutRubric = 10,

    /// <summary>
    /// Выбор корневого, а вложенные по желанию
    /// </summary>
    [Description("Любая рубрика")]
    SelectAny = 20,

    /// <summary>
    /// Строго заполнить всю иерархию
    /// </summary>
    [Description("Строго выбрать всю иерархию")]
    Strict = 30
}