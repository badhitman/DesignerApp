////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OffersOfGoodsComponent
/// </summary>
public partial class OffersOfGoodsComponent : BlazorBusyComponentRegistersModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StorageTransmissionRepo { get; set; } = default!;

    /// <summary>
    /// CurrentGoods
    /// </summary>
    [Parameter, EditorRequired]
    public required GoodsModelDB CurrentGoods { get; set; }


    bool _hideMultiplicity;
    bool HideMultiplicity
    {
        get => _hideMultiplicity;
        set
        {
            _hideMultiplicity = value;
            InvokeAsync(SaveHideMultiplicity);
        }
    }

    bool _hideWorth;
    bool HideWorth
    {
        get => _hideWorth;
        set
        {
            _hideWorth = value;
            InvokeAsync(SaveHideWorth);
        }
    }

    private MudTable<OfferGoodModelDB> table = default!;
    bool _visibleChangeConfig;
    readonly DialogOptions _dialogOptions = new()
    {
        FullWidth = true,
        CloseButton = true
    };

    async void SaveHideWorth()
    {
        await SetBusy();
        TResponseModel<int> res = await StorageTransmissionRepo.SaveParameter<bool?>(HideWorth, GlobalStaticConstants.CloudStorageMetadata.HideWorthOffers, true);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await table.ReloadServerData();
    }

    async void SaveHideMultiplicity()
    {
        await SetBusy();
        TResponseModel<int> res = await StorageTransmissionRepo.SaveParameter<bool?>(HideMultiplicity, GlobalStaticConstants.CloudStorageMetadata.HideMultiplicityOffers, true);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await table.ReloadServerData();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await SetBusy();
        TResponseModel<bool?> res = await StorageTransmissionRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.HideWorthOffers);
        if (!res.Success())
            SnackbarRepo.ShowMessagesResponse(res.Messages);
        else
            _hideWorth = res.Response == true;

        res = await StorageTransmissionRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.HideMultiplicityOffers);
        if (!res.Success())
            SnackbarRepo.ShowMessagesResponse(res.Messages);
        else
            _hideMultiplicity = res.Response == true;

        await SetBusy(false);
    }

    void CancelChangeConfig()
    {
        _visibleChangeConfig = !_visibleChangeConfig;
    }

    async void CreateOfferAction(OfferGoodModelDB sender)
    {
        await table.ReloadServerData();
        OnExpandCollapseClick();
        StateHasChanged();
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<OfferGoodModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        TPaginationRequestModel<OffersSelectRequestModel> req = new()
        {
            Payload = new()
            {
                GoodsFilter = [CurrentGoods.Id]
            },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        await SetBusy(token: token);
        TResponseModel<TPaginationResponseModel<OfferGoodModelDB>> res = await CommerceRepo.OffersSelect(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (res.Success() && res.Response?.Response is not null)
        {
            await CacheRegistersOfferUpdate(res.Response.Response.Select(x => x.Id));
            IsBusyProgress = false;
            return new TableData<OfferGoodModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
        }

        IsBusyProgress = false;
        return new TableData<OfferGoodModelDB>() { TotalItems = 0, Items = [] };
    }

    bool _expanded;
    private void OnExpandCollapseClick()
    {
        _expanded = !_expanded;
    }
}