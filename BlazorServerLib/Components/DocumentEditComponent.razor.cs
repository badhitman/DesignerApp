////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components;

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