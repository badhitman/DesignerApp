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
    public byte[] Data { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
}