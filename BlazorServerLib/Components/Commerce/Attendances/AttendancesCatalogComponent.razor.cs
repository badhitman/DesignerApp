////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// AttendancesCatalogComponent
/// </summary>
public partial class AttendancesCatalogComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    bool _expanded;
    MudTable<NomenclatureModelDB> tableRef = default!;


    async void CreateNomenclatureAction(NomenclatureModelDB nom)
    {
        await tableRef.ReloadServerData();
        OnExpandCollapseClick();
        StateHasChanged();
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<NomenclatureModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        TPaginationRequestModel<NomenclaturesSelectRequestModel> req = new()
        {
            Payload = new() { ContextName = GlobalStaticConstants.Routes.ATTENDANCES_CONTROLLER_NAME },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        await SetBusy(token: token);
        TResponseModel<TPaginationResponseModel<NomenclatureModelDB>> res = await CommerceRepo.NomenclaturesSelect(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        IsBusyProgress = false;

        if (!res.Success() || res.Response?.Response is null)
            return new TableData<NomenclatureModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<NomenclatureModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
    }

    private void OnExpandCollapseClick()
    {
        _expanded = !_expanded;
    }
}