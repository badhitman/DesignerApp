////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ExeCommandModel
/// </summary>
public class ExeCommandModel
{
    /// <summary>
    /// FileName
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Arguments
    /// </summary>
    public required string Arguments { get; set; }
}