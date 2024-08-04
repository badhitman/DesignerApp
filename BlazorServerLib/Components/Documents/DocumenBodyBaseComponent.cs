////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib;

/// <summary>
/// DocumenBodyBaseComponent
/// </summary>
public abstract class DocumenBodyBaseComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    public IJournalUniversalService JournalRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    public ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// ID (html dom)
    /// </summary>
    [Parameter, EditorRequired]
    public required string ID { get; set; }

    /// <summary>
    /// Document Metadata
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required DocumentFitModel DocumentMetadata { get; set; }

    /// <summary>
    /// PK строки БД.
    /// </summary>
    /// <remarks>
    /// Если null, то demo решим. Если HasValue и меньше 1, тогда создание нового объекта
    /// </remarks>
    [Parameter]
    public int? DocumentKey { get; set; }


    /// <summary>
    /// IsEdited
    /// </summary>
    public abstract bool IsEdited { get; }    
}