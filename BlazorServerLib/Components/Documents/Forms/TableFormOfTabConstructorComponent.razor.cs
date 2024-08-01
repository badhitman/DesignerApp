////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Documents.Forms;

/// <summary>
/// TableFormOfTabConstructorComponent
/// </summary>
public partial class TableFormOfTabConstructorComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required List<ValueDataForSessionOfDocumentModelDB> SessionValues { get; set; }

    /// <summary>
    /// Form
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormConstructorModelDB Form {  get; set; }


    /// <summary>
    /// DocumentMetadata
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required DocumentFitModel DocumentMetadata { get; set; }

    /// <summary>
    /// FormMetadata
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormFitModel FormMetadata { get; set; }
}