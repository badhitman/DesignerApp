////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Размеры
/// </summary>
public enum MeasurementsSimpleEnum
{
    /// <summary>
    /// Маленький
    /// </summary>
    [Description("Маленький")]
    Sm,

    /// <summary>
    /// Средний
    /// </summary>
    [Description("Средний")]
    Norm,

    /// <summary>
    /// Большой
    /// </summary>
    [Description("Большой")]
    Lg
}