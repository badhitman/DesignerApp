////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// WarehouseDocumentModelDB
/// </summary>
[Index(nameof(DeliveryData)), Index(nameof(NormalizedUpperName))]
public class WarehouseDocumentModelDB : EntrySwitchableUpdatedModel
{
    /// <summary>
    /// NormalizedUpperName
    /// </summary>
    public required string NormalizedUpperName { get; set; }

    /// <summary>
    /// Дата доставки
    /// </summary>
    [Required]
    public required DateTime DeliveryData { get; set; }

    /// <summary>
    /// Идентификатор документа из внешней системы (например 1С)
    /// </summary>
    public string? ExternalDocumentId { get; set; }

    /// <summary>
    /// Rows
    /// </summary>
    public List<RowOfWarehouseDocumentModelDB>? Rows { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is WarehouseDocumentModelDB _wd)
            return
                Id == _wd.Id &&
                Name == _wd.Name &&
                IsDisabled == _wd.IsDisabled &&
                DeliveryData == _wd.DeliveryData &&
                ExternalDocumentId == _wd.ExternalDocumentId &&
                _wd.Description == Description;

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{Id} {IsDisabled} {DeliveryData} {Name} {ExternalDocumentId} {Description}".GetHashCode();
    }
}