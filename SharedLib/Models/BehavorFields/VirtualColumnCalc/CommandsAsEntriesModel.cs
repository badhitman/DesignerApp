////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// 
/// </summary>
public class CommandsAsEntriesModel
{
    /// <summary>
    /// 
    /// </summary>
    public string CommandName { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<string> Options { get; set; } = default!;
}