////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Documents;

/// <summary>
/// Document edit
/// </summary>
public partial class DocumentEditConstructorComponent : DocumentEditBaseComponent
{
    [Inject]
    IConstructorService ConstructorRepo { get; set; } = default!;


    /// <summary>
    /// Project Id
    /// </summary>
    [Parameter]
    public int? ProjectId { get; set; }


    DocumentSchemeConstructorModelDB[]? schemes;
    SessionOfDocumentDataModelDB? session;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (IsCancel)
            return;

        IsBusyProgress = true;
        TResponseModel<DocumentSchemeConstructorModelDB[]?> ds = await JournalRepo.FindDocumentSchemes(DocumentNameOrId, ProjectId);
        SnackbarRepo.ShowMessagesResponse(ds.Messages);
        IsBusyProgress = false;
        schemes = ds.Response;

        if (schemes?.Length == 1 && DocumentKey.HasValue && DocumentKey.Value > 0)
        {
            TResponseModel<SessionOfDocumentDataModelDB> session_data = await ConstructorRepo.GetSessionDocument(DocumentKey.Value);
            SnackbarRepo.ShowMessagesResponse(session_data.Messages);
            session = session_data.Response;
        }
    }

    /// <inheritdoc/>
    public override Task TabChangeEvent(TTabOfDocumenBaseComponent tab_sender)
    {
        TabOfDocumentConstructorComponent ct = (TabOfDocumentConstructorComponent)tab_sender;

        return Task.CompletedTask;
    }
}