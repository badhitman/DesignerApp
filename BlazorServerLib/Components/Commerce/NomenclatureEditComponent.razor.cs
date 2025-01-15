////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// NomenclatureEditComponent
/// </summary>
public partial class NomenclatureEditComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ICommerceTransmission CommerceRepo { get; set; } = default!;


    /// <summary>
    /// Nomenclature
    /// </summary>
    [Parameter, EditorRequired]
    public required int NomenclatureId { get; set; }


    NomenclatureModelDB? CurrentNomenclature;
    NomenclatureModelDB? editNomenclature;
    FilesContextViewComponent? filesViewRef;

    string images_upload_url = default!;
    Dictionary<string, object> editorConf = default!;

    bool CanSave => CurrentNomenclature is not null && (CurrentNomenclature.Name != editNomenclature?.Name || CurrentNomenclature.Description != editNomenclature?.Description || CurrentNomenclature.BaseUnit != editNomenclature?.BaseUnit);

    async Task SaveNomenclature()
    {
        if (editNomenclature is null)
            throw new ArgumentNullException(nameof(editNomenclature));

        await SetBusy();

        TResponseModel<int> res = await CommerceRepo.NomenclatureUpdateReceive(editNomenclature);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success())
            CurrentNomenclature = GlobalTools.CreateDeepCopy(editNomenclature);

        if (filesViewRef is not null)
            await filesViewRef.ReloadServerData();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        images_upload_url = $"{GlobalStaticConstants.TinyMCEditorUploadImage}{GlobalStaticConstants.Routes.NOMENCLATURE_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.BODY_CONTROLLER_NAME}?{nameof(StorageMetadataModel.PrefixPropertyName)}={GlobalStaticConstants.Routes.IMAGE_ACTION_NAME}&{nameof(StorageMetadataModel.OwnerPrimaryKey)}={NomenclatureId}";
        editorConf = GlobalStaticConstants.TinyMCEditorConf(images_upload_url);

        await base.OnInitializedAsync();
        await SetBusy();
        if (CurrentUserSession is null)
            throw new Exception();

        TResponseModel<List<NomenclatureModelDB>> res = await CommerceRepo.NomenclaturesRead(new() { Payload = [NomenclatureId], SenderActionUserId = CurrentUserSession.UserId });
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        IsBusyProgress = false;

        if (res.Response is not null && res.Response.Count != 0)
        {
            CurrentNomenclature = res.Response.Single();
            editNomenclature = GlobalTools.CreateDeepCopy(CurrentNomenclature);
        }
    }
}