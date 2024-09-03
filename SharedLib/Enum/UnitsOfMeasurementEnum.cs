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
    None = 0,

    /// <summary>
    /// Коробка
    /// </summary>
    [Description("Коробка")]
    Box = 10,

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
    /// Штука
    /// </summary>
    [Description("Штука")]
    Thing = 40,
}