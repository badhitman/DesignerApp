////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Цветовые стили Bootstrap
/// </summary>
public enum BootstrapColorsStylesEnum
{
    /// <summary>
    /// Primary
    /// </summary>
    [Description("Primary")]
    Primary,

    /// <summary>
    /// Secondary
    /// </summary>
    [Description("Secondary")]
    Secondary,

    /// <summary>
    /// Success
    /// </summary>
    [Description("Success")]
    Success,

    /// <summary>
    /// Danger
    /// </summary>
    [Description("Danger")]
    Danger,

    /// <summary>
    /// Warning
    /// </summary>
    [Description("Warning")]
    Warning,

    /// <summary>
    /// Info
    /// </summary>
    [Description("Info")]
    Info,

    /// <summary>
    /// Light
    /// </summary>
    [Description("Light")]
    Light,

    /// <summary>
    /// Dark
    /// </summary>
    [Description("Dark")]
    Dark,

    /// <summary>
    /// Body
    /// </summary>
    [Description("Body")]
    Body,

    /// <summary>
    /// Muted
    /// </summary>
    [Description("Muted")]
    Muted,

    /// <summary>
    /// White
    /// </summary>
    [Description("White")]
    White,

    /// <summary>
    /// Black (50 percent)
    /// </summary>
    [Description("Black (50 percent)")]
    Black50Percent,

    /// <summary>
    /// White (50 percent)
    /// </summary>
    [Description("White (50 percent)")]
    White50Percent
}