////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Режимы работы reCaptcha
/// </summary>
public enum ReCaptchaModesEnum
{
    /// <summary>
    /// Отключено
    /// </summary>
    [Description("Отключено")]
    None = 0,

    /// <summary>
    /// v2
    /// </summary>
    [Description("v2")]
    Version2 = 1,

    /// <summary>
    /// v2 Invisible
    /// </summary>
    [Description("v2 Invisible")]
    Version2Invisible = 2,

    /*/// <summary>
    /// v3
    /// </summary>
    Version3 = 4,*/
}