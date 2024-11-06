////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ToolsFilesResponseModel
/// </summary>
public class ToolsFilesResponseModel
{
    /// <summary>
    /// FullName File
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// ScopeName File
    /// </summary>
    public required string ScopeName { get; set; }

    /// <summary>
    /// Size File
    /// </summary>
    public required long Size { get; set; }

    /// <summary>
    /// Version File
    /// </summary>
    public string? Version { get; set; }

/// <inheritdoc/>
    public override string ToString()
    {
        return $"{GlobalTools.SizeDataAsString(Size)} - ({Version}): {ScopeName}";
    }
}