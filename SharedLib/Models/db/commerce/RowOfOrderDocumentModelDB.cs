////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Строка заказа (документа)
/// </summary>
[Index(nameof(AddressForOrderTabId), nameof(OfferId), IsUnique = true)]
[Index(nameof(AddressForOrderTabId))]
public class RowOfOrderDocumentModelDB : RowOfBaseDocumentModelDB
{
    /// <summary>
    /// Адрес доставки (из документа заказа)
    /// </summary>
    public TabAddressForOrderModelDb? AddressForOrderTab { get; set; }
    /// <summary>
    /// AddressForOrderTab
    /// </summary>
    public int AddressForOrderTabId { get; set; }

    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public OrderDocumentModelDB? OrderDocument { get; set; }

    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public int? OrderDocumentId { get; set; }

    /// <summary>
    /// Сумма
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Version
    /// </summary>
    [ConcurrencyCheck]
    public Guid Version { get; set; }
}