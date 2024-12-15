////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Единицы
/// </summary>
public enum UnitsOfMeasurementEnum
{
    /// <summary>
    /// Штука
    /// </summary>
    [Description("шт.")]
    Thing = 0,

    /// <summary>
    /// Кипа
    /// </summary>
    [Description("Кипа")]
    Stack = 20,

    /// <summary>
    /// Связка
    /// </summary>
    [Description("Связка")]
    Bunch = 30,

    /// <summary>
    /// Коробка
    /// </summary>
    [Description("Коробка")]
    Box = 40,

    /// <summary>
    /// None
    /// </summary>
    [Description("-нет-")]
    None = int.MaxValue,
}