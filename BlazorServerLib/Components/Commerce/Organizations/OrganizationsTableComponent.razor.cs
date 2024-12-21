////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Organizations;

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
            await SetBusy();
            TResponseModel<UserInfoModel[]?> user_res = await WebRepo.GetUsersIdentity([UserId]);
            SnackbarRepo.ShowMessagesResponse(user_res.Messages);
            CurrentViewUser = user_res.Response?.FirstOrDefault();
            await SetBusy(false);
        }
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<OrganizationModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        if (CurrentUserSession is null)
            return new TableData<OrganizationModelDB>() { TotalItems = 0, Items = [] };

        TPaginationRequestAuthModel<OrganizationsSelectRequestModel> req = new()
        {
            Payload = new()
            {
                ForUserIdentityId = _filterUser,
            },
            SenderActionUserId = CurrentUserSession.UserId,
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        await SetBusy(token: token);
        TResponseModel<TPaginationResponseModel<OrganizationModelDB>> res = await CommerceRepo.OrganizationsSelect(req);
        await SetBusy(false, token: token);
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (!res.Success() || res.Response?.Response is null)
            return new TableData<OrganizationModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<OrganizationModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
    }
}