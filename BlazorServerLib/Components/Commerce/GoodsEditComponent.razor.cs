////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// GoodsEditComponent
/// </summary>
public partial class GoodsEditComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// GoodsId
    /// </summary>
    [Parameter, EditorRequired]
    public required int GoodsId { get; set; }


    NomenclatureModelDB? CurrentGoods;
    NomenclatureModelDB? editGoods;
    FilesContextViewComponent? filesViewRef;

    string images_upload_url = default!;
    Dictionary<string, object> editorConf = default!;

    bool CanSave => CurrentGoods is not null && (CurrentGoods.Name != editGoods?.Name || CurrentGoods.Description != editGoods?.Description || CurrentGoods.BaseUnit != editGoods?.BaseUnit);

    async Task SaveGoods()
    {
        if (editGoods is null)
            throw new ArgumentNullException(nameof(editGoods));

        await SetBusy();

        TResponseModel<int> res = await CommerceRepo.GoodUpdateReceive(editGoods);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success())
            CurrentGoods = GlobalTools.CreateDeepCopy(editGoods);

        if (filesViewRef is not null)
            await filesViewRef.ReloadServerData();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        images_upload_url = $"{GlobalStaticConstants.TinyMCEditorUploadImage}{GlobalStaticConstants.Routes.GOODS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.BODY_CONTROLLER_NAME}?{nameof(StorageMetadataModel.PrefixPropertyName)}={GlobalStaticConstants.Routes.IMAGE_ACTION_NAME}&{nameof(StorageMetadataModel.OwnerPrimaryKey)}={GoodsId}";
        editorConf = GlobalStaticConstants.TinyMCEditorConf(images_upload_url);

        await ReadCurrentUser();
        await SetBusy();
        if (CurrentUserSession is null)
            throw new Exception();

        TResponseModel<NomenclatureModelDB[]> res = await CommerceRepo.GoodsRead([GoodsId]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success() && res.Response is not null && res.Response.Length != 0)
        {
            CurrentGoods = res.Response.Single();
            editGoods = GlobalTools.CreateDeepCopy(CurrentGoods);
        }
    }
}