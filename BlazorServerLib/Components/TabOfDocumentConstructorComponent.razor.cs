////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components;

/// <summary>
/// TabOfDocumentConstructorComponent
/// </summary>
public partial class TabOfDocumentConstructorComponent : TTabOfDocumenBaseComponent
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required int DocumentKey { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required int TabId { get; set; }


    /// <summary>
    /// Session values
    /// </summary>
    protected ValueDataForSessionOfDocumentModelDB[] SessionValues { get; private set; } = default!;


    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        SessionValues = await JournalRepo.ReadSessionTabValues(TabId, DocumentKey);
        IsBusyProgress = false;
        await base.OnInitializedAsync();
    }
}