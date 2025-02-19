﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// StorageMetadataModel
/// </summary>
[Index(nameof(PrefixPropertyName), nameof(OwnerPrimaryKey))]
public class StorageMetadataModel : RequestStorageBaseModel
{
    /// <summary>
    /// Префикс имени (опционально)
    /// </summary>
    public string? PrefixPropertyName { get; set; }

    /// <summary>
    /// Связанный PK строки базы данных (опционально)
    /// </summary>
    public int? OwnerPrimaryKey { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        Normalize();
        return $"{PrefixPropertyName}:{OwnerPrimaryKey}:{PropertyName}:{ApplicationName}";
    }

    /// <summary>
    /// Normalize
    /// </summary>
    public override void Normalize()
    {
        base.Normalize();
        PrefixPropertyName = PrefixPropertyName?.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
    }
}