////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components;

/// <summary>
/// TabOfDocumentComponent
/// </summary>
public abstract class TTabOfDocumenBaseComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    public IJournalUniversalService JournalRepo { get; set; } = default!;

    /// <summary>
    /// Формы в табе
    /// </summary>
    public List<FormBaseModel> FormsStack { get; set; } = [];

    /// <summary>
    /// Tab Metadata
    /// </summary>
    [Parameter, EditorRequired]
    public required TabFitModel TabMetadata { get; set; }

    /// <summary>
    /// Document Metadata
    /// </summary>
    [Parameter, EditorRequired]
    public required DocumentFitModel DocumentMetadata { get; set; }
}