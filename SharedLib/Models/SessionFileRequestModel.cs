////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SessionFileRequestModel
/// </summary>
public class SessionFileRequestModel(string sessionId, string fileId, byte[] data, string fileName, uint partFileIndex) : SessionFileTokensModel(sessionId, fileId)
{
    /// <summary>
    /// Data
    /// </summary>
    public byte[] Data { get; init; } = data;

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; } = fileName;

    /// <summary>
    /// Номер (порядковый) порции данных
    /// </summary>
    public uint PartFileIndex { get; set; } = partFileIndex;
}