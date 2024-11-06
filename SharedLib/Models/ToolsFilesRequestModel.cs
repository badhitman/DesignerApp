////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ToolsFilesRequestModel
/// </summary>
public class ToolsFilesRequestModel
{
    /// <summary>
    /// RemoteDirectory
    /// </summary>
    public required string RemoteDirectory { get; set; }

    /// <summary>
    /// CalculationVersion
    /// </summary>
    public bool CalculationVersion { get; set; }
}