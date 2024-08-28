////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// FileAttachTelegramModel
/// </summary>
public class FileAttachTelegramModel
{
    /// <summary>
    /// Data
    /// </summary>
    public required byte[] Data { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string ContentType { get; set; }
}