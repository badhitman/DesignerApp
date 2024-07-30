////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components;

/// <summary>
/// Document edit
/// </summary>
public partial class DocumentEditComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IJournalUniversalService JournalRepo { get; set; } = default!;


    /// <summary>
    /// Тип документа
    /// </summary>
    [Parameter, EditorRequired]
    public required string DocumentNameOrId { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    [Parameter, EditorRequired]
    public required int DocumentId { get; set; }


}