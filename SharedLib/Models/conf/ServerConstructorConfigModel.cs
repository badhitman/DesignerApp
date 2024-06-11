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
    /// Срок жизни создаваемой сессии опроса/анкеты (в минутах)
    /// </summary>
    public int TimeActualityQuestionnaireSessionMinutes { get; set; }
}
