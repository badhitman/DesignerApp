////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Typed storage cloud parameter
/// </summary>
public class TStorageCloudParameterModel<T> : StorageMetadataModel
{
    /// <summary>
    /// Payload
    /// </summary>
    public T? Payload { get; set; }
}