////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Серверные конфигурации
/// </summary>
public class ServerConfigModel
{
    /// <summary>
    /// Путь к папке для хранения файлов программы
    /// </summary>
    public string RootFolderName { get; set; } = "./storage-files";

    /// <summary>
    /// Конфигурация web сервера
    /// </summary>
    public BackendConfigModel KastrelConfig { get; set; } = new BackendConfigModel();

    /// <summary>
    /// Конфигурация reCaptcha
    /// </summary>
    public ReCaptchaConfigClientModel? ReCaptchaConfig { get; set; }
}