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

    /// <summary>
    /// Удалить все предыдущие значения (очистить историю)
    /// </summary>
    public bool TrimHistory { get; set; }
}