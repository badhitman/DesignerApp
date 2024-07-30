////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components;

/// <summary>
/// Document edit
/// </summary>
public partial class DocumentEditConstructorComponent : DocumentEditBaseComponent
{
    /// <summary>
    /// Project Id
    /// </summary>
    [Parameter]
    public int? ProjectId { get; set; }

    DocumentSchemeConstructorModelDB[]? schemes;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (_is_return)
            return;

        IsBusyProgress = true;
        TResponseModel<DocumentSchemeConstructorModelDB[]?> ds = await JournalRepo.FindDocumentSchemes(DocumentNameOrId, ProjectId);
        schemes = ds.Response;
        IsBusyProgress = false;
    }
}