////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Метаданные логов: доступные типы данных и общая статистика по ним
/// </summary>
public class LogsMetadataResponseModel : PeriodDatesTimesModel
{
    /// <inheritdoc/>
    public required Dictionary<string, int> LevelsAvailable { get; set; }

    /// <inheritdoc/>
    public required Dictionary<string, int> ApplicationsAvailable { get; set; }

    /// <inheritdoc/>
    public required Dictionary<string, int> ContextsPrefixesAvailable { get; set; }

    /// <inheritdoc/>
    public required Dictionary<string, int> LoggersAvailable { get; set; }
}