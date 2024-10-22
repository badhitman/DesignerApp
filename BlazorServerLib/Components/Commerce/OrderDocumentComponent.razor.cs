////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OrderDocumentComponent
/// </summary>
public partial class OrderDocumentComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    IWebRemoteTransmissionService WebRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// Заказ
    /// </summary>
    [Parameter, EditorRequired]
    public required int DocumentId { get; set; }
}