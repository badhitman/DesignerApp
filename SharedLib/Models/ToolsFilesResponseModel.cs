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
    /// SafeScopeName
    /// </summary>
    public string SafeScopeName { get 
        {
            string _res = ScopeName;

            while (_res.StartsWith("\\") || _res.StartsWith("/"))
                _res = _res[1..];

            return _res;
        } 
    }

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
        return $"{GlobalTools.SizeDataAsString(Size)} - {(string.IsNullOrWhiteSpace(Version) ? "" : $"({Version})")}: {ScopeName}";
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is ToolsFilesResponseModel _t)
            return _t.ScopeName == ScopeName && _t.Size == Size && (_t.Version == Version || (string.IsNullOrWhiteSpace(_t.Version) && string.IsNullOrWhiteSpace(Version)));

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{Size} {Version} {ScopeName}".GetHashCode();
    }
}