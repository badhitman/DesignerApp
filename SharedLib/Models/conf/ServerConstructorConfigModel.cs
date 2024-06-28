////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Серверные конфигурации
/// </summary>
public class ServerConstructorConfigModel : ServerConfigModel
{
    /// <summary>
    /// Срок жизни создаваемой сессии документа (в минутах)
    /// </summary>
    public int TimeActualityDocumentSessionMinutes { get; set; }
}
