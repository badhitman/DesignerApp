////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <inheritdoc/>
[Index(nameof(CreatedAt))]
public class StorageBaseModelDB : StorageMetadataModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Создание
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;    
}