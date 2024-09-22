////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OrdersListComponent
/// </summary>
public partial class OrdersListComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Orders
    /// </summary>
    [Parameter, EditorRequired]
    public required OrderDocumentModelDB[] Orders { get; set; }
}