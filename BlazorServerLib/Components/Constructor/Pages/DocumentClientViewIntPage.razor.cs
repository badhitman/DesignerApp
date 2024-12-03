////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Pages;

/// <inheritdoc/>
public partial class DocumentClientViewIntPage : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public int DocumentId { get; set; }


    SessionOfDocumentDataModelDB SessionDocument = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await SetBusy();
        await ReadCurrentUser();
        
        TResponseModel<SessionOfDocumentDataModelDB> rest = await ConstructorRepo.GetSessionDocument(new() { SessionId = DocumentId, IncludeExtra = true });
        IsBusyProgress = false;

        if (rest.Response is null)
            throw new Exception("rest.Content.SessionDocument is null. error 09DFC142-55DA-4616-AA14-EA1B810E9A7E");

        SessionDocument = rest.Response;
        if (SessionDocument.DataSessionValues is not null && SessionDocument.DataSessionValues.Count != 0)
            SessionDocument.DataSessionValues.ForEach(x => { x.Owner ??= SessionDocument; x.OwnerId = SessionDocument.Id; });
    }
}