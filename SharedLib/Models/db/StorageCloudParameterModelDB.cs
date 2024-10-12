////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <inheritdoc/>
[Index(nameof(TypeName))]
public class StorageCloudParameterModelDB : StorageBaseModelDB
{
    /// <summary>
    /// Данные (сериализованные)
    /// </summary>
    public required string SerializedDataJson { get; set; }

    /// <summary>
    /// Тип сериализуемого параметра
    /// </summary>
    public required string TypeName { get; set; }
}