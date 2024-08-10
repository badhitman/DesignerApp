////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <inheritdoc/>
public class StorageCloudParameterModelDB : StorageCloudParameterModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Данные (сериализованные)
    /// </summary>
    public required string SerializedDataJson { get; set; }

    /// <summary>
    /// Создание
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Тип сериализуемого параметра
    /// </summary>
    public required string TypeName { get; set; }
}