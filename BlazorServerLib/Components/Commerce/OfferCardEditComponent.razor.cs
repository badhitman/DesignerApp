////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OfferCardEditComponent
/// </summary>
public partial class OfferCardEditComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// OfferId
    /// </summary>
    [Parameter, EditorRequired]
    public int OfferId { get; set; }


    OfferGoodModelDB CurrentOffer = default!;
    OfferGoodModelDB editOffer = default!;
    FilesContextViewComponent? filesViewRef;
    string images_upload_url = default!;
    Dictionary<string, object> editorConf = default!;

    bool CanSave =>
        editOffer.IsDisabled != CurrentOffer.IsDisabled ||
        editOffer.ShortName != CurrentOffer.ShortName ||
        editOffer.Name != CurrentOffer.Name ||
        editOffer.QuantitiesTemplate != CurrentOffer.QuantitiesTemplate ||
        editOffer.Price != CurrentOffer.Price ||
        editOffer.Multiplicity != CurrentOffer.Multiplicity ||
        editOffer.OfferUnit != CurrentOffer.OfferUnit;

    async Task SaveOffer()
    {
        await SetBusy();

        TResponseModel<int> res = await CommerceRepo.OfferUpdate(editOffer);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success() && res.Response > 0)
            CurrentOffer = GlobalTools.CreateDeepCopy(editOffer)!;

        if (filesViewRef is not null)
            await filesViewRef.ReloadServerData();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        images_upload_url = $"{GlobalStaticConstants.TinyMCEditorUploadImage}{GlobalStaticConstants.Routes.OFFER_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.BODY_CONTROLLER_NAME}?{nameof(StorageMetadataModel.PrefixPropertyName)}={GlobalStaticConstants.Routes.IMAGE_ACTION_NAME}&{nameof(StorageMetadataModel.OwnerPrimaryKey)}={OfferId}";
        editorConf = GlobalStaticConstants.TinyMCEditorConf(images_upload_url);

        await SetBusy();
        await ReadCurrentUser();
        TResponseModel<OfferGoodModelDB[]> res = await CommerceRepo.OffersRead([OfferId]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        CurrentOffer = res.Response!.Single();
        editOffer = GlobalTools.CreateDeepCopy(CurrentOffer)!;
    }
}