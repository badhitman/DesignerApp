﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Строка заказа (документа)
/// </summary>
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
}