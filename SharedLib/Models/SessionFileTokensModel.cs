////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SessionFileTokensModel
/// </summary>
public class SessionFileTokensModel(string sessionId, string fileId)
{
    /// <summary>
    /// Session
    /// </summary>
    public string SessionId { get; init; } = sessionId;

    /// <summary>
    /// File
    /// </summary>
    public string FileId { get; init; } = fileId;
}