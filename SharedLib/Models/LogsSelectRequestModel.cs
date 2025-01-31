////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// LogsSelectRequestModel
/// </summary>
public class LogsSelectRequestModel
{
    /// <summary>
    /// LevelsFilter
    /// </summary>
    public string[]? LevelsFilter { get; set; }

    /// <summary>
    /// ApplicationsFilter
    /// </summary>
    public string[]? ApplicationsFilter { get; set; }

    /// <summary>
    /// ContextsPrefixesFilter
    /// </summary>
    public string[]? ContextsPrefixesFilter { get; set; }

    /// <summary>
    /// LoggersFilter
    /// </summary>
    public string[]? LoggersFilter { get; set; }
}