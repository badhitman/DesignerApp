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
    /// Document Metadata
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required DocumentFitModel DocumentMetadata { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [CascadingParameter]
    public int? DocumentKey { get; set; }
}