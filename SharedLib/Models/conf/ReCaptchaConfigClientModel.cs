////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Конфигурация reCaptcha
/// </summary>
public class ReCaptchaConfigClientModel
{
    /// <summary>
    /// Режим работы reCaptcha
    /// </summary>
    public ReCaptchaModesEnum Mode { get; set; } = ReCaptchaModesEnum.None;

    /// <summary>
    /// Настройки reCaptcha V2
    /// </summary>
    public KeysPairsConfigClientModel ReCaptchaV2Config { get; set; } = new KeysPairsConfigClientModel();

    /// <summary>
    /// Настройки reCaptcha V2 Invisible
    /// </summary>
    public KeysPairsConfigClientModel ReCaptchaV2InvisibleConfig { get; set; } = new KeysPairsConfigClientModel();

    /*public KeysPairsConfigClientModel ReCaptchaV3Config { get; set; } = new();*/
}