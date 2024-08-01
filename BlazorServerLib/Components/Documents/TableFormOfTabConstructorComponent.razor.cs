////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Documents;

/// <summary>
/// TableFormOfTabConstructorComponent
/// </summary>
public partial class TableFormOfTabConstructorComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required IEnumerable<ValueDataForSessionOfDocumentModelDB> SessionValues { get; set; }
}