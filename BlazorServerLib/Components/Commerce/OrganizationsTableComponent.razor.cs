////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OrganizationsTableComponent
/// </summary>
public partial class OrganizationsTableComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService WebRepo { get; set; } = default!;


    /// <summary>
    /// Пользователь, для которого отобразить организации
    /// </summary>
    [Parameter]
    public string? UserId { get; set; }


    string? _filterUser;

    UserInfoMainModel? CurrentViewUser;

    private MudTable<OrganizationModelDB> table = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReadCurrentUser();
        if (string.IsNullOrWhiteSpace(UserId))
        {
            if (CurrentUserSession!.IsAdmin)
                _filterUser = UserId;
            else
            {
                _filterUser = CurrentUserSession!.UserId;
                CurrentViewUser = CurrentUserSession!;
            }
        }
        else
        {
            if (CurrentUserSession!.IsAdmin || CurrentUserSession!.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Contains(x)) == true)
                _filterUser = UserId;
            else
                _filterUser = CurrentUserSession!.UserId;
        }

        if (UserId == CurrentUserSession!.UserId)
            CurrentViewUser = CurrentUserSession!;
        else if (!string.IsNullOrWhiteSpace(UserId))
        {
            SetBusy();

            TResponseModel<UserInfoModel[]?> user_res = await WebRepo.GetUsersIdentity([UserId]);
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(user_res.Messages);
            CurrentViewUser = user_res.Response?.FirstOrDefault();
        }
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<OrganizationModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        TPaginationRequestModel<OrganizationsSelectRequestModel> req = new()
        {
            Payload = new()
            {
                ForUserIdentityId = _filterUser,
            },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        SetBusy();

        TResponseModel<TPaginationResponseModel<OrganizationModelDB>> res = await CommerceRepo.OrganizationsSelect(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (!res.Success() || res.Response?.Response is null)
            return new TableData<OrganizationModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<OrganizationModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
    }
}