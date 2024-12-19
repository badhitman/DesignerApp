﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// AttendancesExecutorsComponent
/// </summary>
public partial class AttendancesExecutorsComponent : BlazorBusyComponentUsersCachedModel
{
    /// <summary>
    /// Commerce
    /// </summary>
    [Inject]
    protected ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// Offer
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required OfferModelDB? Offer { get; set; }


    MudTable<UserOrganizationModelDB> tableRef = default!;
    readonly Dictionary<string, UsersOrganizationsStatusesEnum> UsersOrganizationsStatuses = [];

    private IEnumerable<string>? _options = [UsersOrganizationsStatusesEnum.None.DescriptionInfo()];
    private IEnumerable<string>? Options
    {
        get => _options;
        set
        {
            _options = value;
            InvokeAsync(tableRef.ReloadServerData);
        }
    }

    private UserOrganizationModelDB? elementBeforeEdit;
    private void BackupItem(object element)
    {
        if (CurrentUserSession is null)
            return;

        UserOrganizationModelDB sender = (UserOrganizationModelDB)element;

        elementBeforeEdit = new()
        {
            Id = sender.Id,
            UserPersonIdentityId = CurrentUserSession.UserId,
            OrganizationId = sender.OrganizationId,
            UserStatus = sender.UserStatus,
            Organization = sender.Organization,
        };
    }

    string GetCssColor(UsersOrganizationsStatusesEnum stat)
    {
        return stat switch
        {
            UsersOrganizationsStatusesEnum.None => "text-warning",
            UsersOrganizationsStatusesEnum.SimpleUnit => "text-success-emphasis",
            UsersOrganizationsStatusesEnum.Manager => "text-primary",
            _ => "text-danger"
        };
    }

    async void ItemHasBeenCommitted(object element)
    {
        if (CurrentUserSession is null)
            return;

        UserOrganizationModelDB sender = (UserOrganizationModelDB)element;
        await SetBusy();
        TResponseModel<int> res = await CommerceRepo.UserOrganizationUpdate(new TAuthRequestModel<UserOrganizationModelDB>() { Payload = sender, SenderActionUserId = CurrentUserSession.UserId });
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await tableRef.ReloadServerData();
    }

    private void ResetItemToOriginalValues(object element)
    {
        if (elementBeforeEdit is null)
            return;

        ((UserOrganizationModelDB)element).Id = elementBeforeEdit.Id;
        ((UserOrganizationModelDB)element).UserStatus = elementBeforeEdit.UserStatus;
        ((UserOrganizationModelDB)element).Organization = elementBeforeEdit.Organization;
        ((UserOrganizationModelDB)element).OrganizationId = elementBeforeEdit.OrganizationId;
        ((UserOrganizationModelDB)element).UserPersonIdentityId = elementBeforeEdit.UserPersonIdentityId;
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        foreach (UsersOrganizationsStatusesEnum uos in Enum.GetValues<UsersOrganizationsStatusesEnum>())
            UsersOrganizationsStatuses.Add(uos.DescriptionInfo(), uos);
    }

    private async Task<TableData<UserOrganizationModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        if (CurrentUserSession is null)
            return new TableData<UserOrganizationModelDB>() { TotalItems = 0, Items = [] };

        TPaginationRequestAuthModel<UsersOrganizationsStatusesRequest> req = new()
        {
            Payload = new()
            {
                UsersOrganizationsFilter = Options?.Select(x => UsersOrganizationsStatuses[x]).ToArray(),
                IncludeExternalData = true,
            },
            SenderActionUserId = CurrentUserSession.UserId,
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        await SetBusy(token: token);
        TResponseModel<TPaginationResponseModel<UserOrganizationModelDB>> res = await CommerceRepo.UsersOrganizationsSelect(req);
        //await SetBusy(false, token);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (!res.Success() || res.Response is null)
            return new TableData<UserOrganizationModelDB>() { TotalItems = 0, Items = [] };

        await CacheUsersUpdate(res.Response.Response.Select(x => x.UserPersonIdentityId));

        // Return the data
        return new TableData<UserOrganizationModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
    }
}