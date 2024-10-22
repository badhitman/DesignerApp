////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Pages;

/// <summary>
/// 
/// </summary>
public partial class DocumentClientViewPage : BlazorBusyComponentBaseModel
{
    [Inject]
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public Guid DocumentGuid { get; set; } = default!;


    SessionOfDocumentDataModelDB SessionDocument = default!;


    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        SetBusy();
        
        TResponseModel<SessionOfDocumentDataModelDB> rest = await ConstructorRepo.GetSessionDocumentData(DocumentGuid.ToString());
        IsBusyProgress = false;

        if (rest.Response is null)
            throw new Exception("rest.SessionDocument is null. error 5E20961A-3F1A-4409-9481-FA623F818918");

        SessionDocument = rest.Response;
        if (SessionDocument.DataSessionValues is not null && SessionDocument.DataSessionValues.Count != 0)
            SessionDocument.DataSessionValues.ForEach(x => { x.Owner ??= SessionDocument; x.OwnerId = SessionDocument.Id; });
    }
}