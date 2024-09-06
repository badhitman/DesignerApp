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
    /// None
    /// </summary>
    [Description("-нет-")]
    None = 0,

    /// <summary>
    /// Штука
    /// </summary>
    [Description("шт.")]
    Thing = 10,

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
}