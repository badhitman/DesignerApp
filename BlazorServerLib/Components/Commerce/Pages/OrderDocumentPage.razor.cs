////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;

namespace BlazorWebLib.Components.Commerce.Pages;

/// <summary>
/// OrderDocumentPage
/// </summary>
public partial class OrderDocumentPage : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Заказ
    /// </summary>
    [Parameter]
    public int? DocumentId { get; set; }
}