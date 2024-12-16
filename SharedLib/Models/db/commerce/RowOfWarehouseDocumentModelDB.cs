////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// RowOfWarehouseDocumentModelDB
/// </summary>
[Index(nameof(WarehouseDocumentId), nameof(OfferId), IsUnique = true)]
[Index(nameof(WarehouseDocumentId))]
public class RowOfWarehouseDocumentModelDB : RowOfMiddleDocumentModel
{
    /// <summary>
    /// WarehouseDocument
    /// </summary>
    public WarehouseDocumentModelDB? WarehouseDocument { get; set; }
    /// <summary>
    /// WarehouseDocument
    /// </summary>
    public int WarehouseDocumentId { get; set; }

    /// <summary>
    /// Version
    /// </summary>
    [ConcurrencyCheck]
    public Guid Version { get; set; }
}