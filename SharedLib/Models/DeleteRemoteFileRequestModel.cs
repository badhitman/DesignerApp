////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// DeleteRemoteFileRequestModel
/// </summary>
public class DeleteRemoteFileRequestModel
{
    /// <summary>
    /// RemoteDirectory
    /// </summary>
    public required string RemoteDirectory { get; set; }

    /// <summary>
    /// SafeScopeName
    /// </summary>
    public required string SafeScopeName { get; set; }
}