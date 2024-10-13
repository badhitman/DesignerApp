////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Серверные конфигурации
/// </summary>
public class ServerConfigModel
{
    /// <summary>
    /// Конфигурация web сервера
    /// </summary>
    public BackendConfigModel KestrelConfig { get; set; } = new BackendConfigModel();

    /// <summary>
    /// Конфигурация reCaptcha
    /// </summary>
    public ReCaptchaConfigClientModel? ReCaptchaConfig { get; set; }
}