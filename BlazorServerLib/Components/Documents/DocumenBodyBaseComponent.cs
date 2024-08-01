////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib;

/// <summary>
/// DocumenBodyBaseComponent
/// </summary>
public abstract class DocumenBodyBaseComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    public IJournalUniversalService JournalRepo { get; set; } = default!;


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