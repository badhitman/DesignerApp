////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OffersComponent
/// </summary>
public partial class OffersComponent : BlazorBusyComponentRegistersModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StorageTransmissionRepo { get; set; } = default!;

    /// <summary>
    /// CurrentNomenclature
    /// </summary>
    [Parameter, EditorRequired]
    public required NomenclatureModelDB CurrentNomenclature { get; set; }


    bool _hideMultiplicity;
    bool _hideWorth;

    private MudTable<OfferModelDB> table = default!;
    bool _visibleChangeConfig;
    readonly DialogOptions _dialogOptions = new()
    {
        FullWidth = true,
        CloseButton = true
    };

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await SetBusy();

        List<Task> tasks = [
            Task.Run(async () => { TResponseModel<bool?> res = await StorageTransmissionRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.HideWorthOffers); if (!res.Success()) SnackbarRepo.ShowMessagesResponse(res.Messages); else _hideWorth = res.Response == true; }),
            Task.Run(async () => { TResponseModel<bool?> res = await StorageTransmissionRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.HideMultiplicityOffers); if (!res.Success()) SnackbarRepo.ShowMessagesResponse(res.Messages); else _hideMultiplicity = res.Response == true;})];

        await Task.WhenAll(tasks);

        await SetBusy(false);
    }

    void CancelChangeConfig()
    {
        _visibleChangeConfig = !_visibleChangeConfig;
    }

    async void CreateOfferAction(OfferModelDB sender)
    {
        await table.ReloadServerData();
        OnExpandCollapseClick();
        StateHasChanged();
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<OfferModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        TPaginationRequestModel<OffersSelectRequestModel> req = new()
        {
            Payload = new()
            {
                NomenclatureFilter = [CurrentNomenclature.Id],
                ContextName = CurrentNomenclature.ContextName,
            },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        await SetBusy(token: token);
        TResponseModel<TPaginationResponseModel<OfferModelDB>> res = await CommerceRepo.OffersSelect(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (res.Success() && res.Response?.Response is not null)
        {
            await CacheRegistersUpdate(offers: res.Response.Response.Select(x => x.Id),goods: []);
            IsBusyProgress = false;
            return new TableData<OfferModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
        }

        IsBusyProgress = false;
        return new TableData<OfferModelDB>() { TotalItems = 0, Items = [] };
    }

    bool _expanded;
    private void OnExpandCollapseClick()
    {
        _expanded = !_expanded;
    }
}