////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.OrderDocumentObject;

/// <summary>
/// RowOfOrderDocumentComponent
/// </summary>
public partial class RowOfOrderDocumentComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommRepo { get; set; } = default!;


    /// <summary>
    /// Строка заказа (документа)
    /// </summary>
    [Parameter, EditorRequired]
    public required RowOfOrderDocumentModelDB Row { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required IssueHelpdeskModelDB Issue { get; set; }
}