////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Настройка reCaptcha
/// </summary>
public class ReCaptchaConfigServerModel : ReCaptchaConfigClientModel
{
    /// <summary>
    /// Конфигурация reCaptcha V2
    /// </summary>
    public new KeysPairsConfigServerModel ReCaptchaV2Config { get; set; } = new KeysPairsConfigServerModel();

    /// <summary>
    /// Конфигурация reCaptcha V2 Invisible
    /// </summary>
    public new KeysPairsConfigServerModel ReCaptchaV2InvisibleConfig { get; set; } = new KeysPairsConfigServerModel();

    /*public KeysPairsConfigServerModel ReCaptchaV3Config { get; set; } = new KeysPairsConfigModel();*/
}