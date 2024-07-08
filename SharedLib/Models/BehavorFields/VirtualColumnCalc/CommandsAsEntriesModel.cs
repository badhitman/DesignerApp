////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// CommandsAsEntries
/// </summary>
public class CommandsAsEntriesModel
{
    /// <summary>
    /// CommandName
    /// </summary>
    public required string CommandName { get; set; }

    /// <summary>
    /// Options
    /// </summary>
    public required IEnumerable<string> Options { get; set; }
}