////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OrganizationsTableComponent
/// </summary>
public partial class OrganizationsTableComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService WebRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;


    /// <summary>
    /// Пользователь, для которого отобразить организации
    /// </summary>
    [Parameter]
    public string? UserId { get; set; }


    string? _filterUser;

    UserInfoMainModel? current_user;

    private IEnumerable<OrganizationModelDB> pagedData = default!;
    private MudTable<OrganizationModelDB> table = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        UserInfoMainModel me = state.User.ReadCurrentUserInfo() ?? throw new Exception();

        if ((string.IsNullOrWhiteSpace(UserId) || _filterUser != me.UserId) && !me.IsAdmin && me.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Contains(x)) != true)
        {
            _filterUser = me.UserId;
            current_user = me;
        }
        else
            _filterUser = UserId;

        if (UserId == me.UserId)
            current_user = me;
        else if (!string.IsNullOrWhiteSpace(UserId))
        {
            IsBusyProgress = true;
            TResponseModel<UserInfoModel[]?> user_res = await WebRepo.GetUsersIdentity([UserId]);
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(user_res.Messages);
            current_user = user_res.Response?.FirstOrDefault();
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
        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<OrganizationModelDB>?> res = await CommerceRepo.OrganizationsSelect(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (!res.Success() || res.Response?.Response is null)
            return new TableData<OrganizationModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<OrganizationModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
    }
}