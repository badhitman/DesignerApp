////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// FileAttachModel
/// </summary>
public class FileAttachModel
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