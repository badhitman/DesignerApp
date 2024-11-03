////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Серверные конфигурации
/// </summary>
public class ConstructorConfigModel
{
    /// <summary>
    /// Срок жизни создаваемой сессии документа (в минутах)
    /// </summary>
    public int TimeActualityDocumentSessionMinutes { get; set; }

    /// <summary>
    /// WebConfig
    /// </summary>
    public TelegramBotConfigModel? WebConfig {  get; set; }
}
