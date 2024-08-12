////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <inheritdoc/>
public class StorageCloudParameterPayloadModel : StorageCloudParameterReadModel
{
    /// <summary>
    /// Данные (сериализованные)
    /// </summary>
    public required string SerializedDataJson { get; set; }
}