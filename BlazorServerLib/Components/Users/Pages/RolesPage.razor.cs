////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Users.Pages;

/// <summary>
/// RolesPage
/// </summary>
public partial class RolesPage
{
    [Inject]
    IUsersProfilesService UsersManageRepo { get; set; } = default!;

    [Inject]
    IIdentityTransmission IdentityRepo { get; set; } = default!;

    /// <summary>
    /// OwnerUserId
    /// </summary>
    [Parameter]
    public string? OwnerUserId { get; set; }

    PaginationState pagination = new() { ItemsPerPage = 15 };
    string nameFilter = string.Empty;
    QuickGrid<RoleInfoModel>? myGrid;
    int numResults;
    GridItemsProvider<RoleInfoModel>? foodRecallProvider;
    string? added_role_name;
    IEnumerable<ResultMessage>? Messages;
    UserInfoModel? UserInfo;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrWhiteSpace(OwnerUserId))
        {
            TResponseModel<UserInfoModel[]> findUsers = await IdentityRepo.GetUsersIdentity([OwnerUserId]);
            Messages = findUsers.Messages;
            if (!findUsers.Success() || findUsers.Response is null)
                return;

            UserInfo = findUsers.Response.Single();
        }

        foodRecallProvider = async req =>
        {
            TPaginationResponseModel<RoleInfoModel> res = await IdentityRepo.FindRolesAsync(new FindWithOwnedRequestModel()
            {
                OwnerId = OwnerUserId,
                FindQuery = nameFilter,
                PageNum = pagination.CurrentPageIndex,
                PageSize = pagination.ItemsPerPage
            });

            if (res.Response is null)
            {
                string msg = "Roles is null. error {CEB87925-CA09-46EE-BCA4-93B7A5DF52EE}";
                throw new Exception(msg);
            }

            if (numResults != res.TotalRowsCount)
            {
                numResults = res.TotalRowsCount;
                StateHasChanged();
            }

            return GridItemsProviderResult.From<RoleInfoModel>(res.Response, res.TotalRowsCount);
        };
    }

    async Task DeleteRole(string? roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = "удаление роли без имени" }];
            return;
        }

        ResponseBaseModel rest = !string.IsNullOrEmpty(UserInfo?.Email)
        ? await IdentityRepo.DeleteRoleFromUser(new() { RoleName = roleName, Email = UserInfo.Email })
        : await IdentityRepo.DeleteRole(roleName.Trim());

        Messages = rest.Messages;
        if (!rest.Success())
            return;
        added_role_name = "";

        if (myGrid is not null)
            await myGrid.RefreshDataAsync();
    }

    async Task AddNewRole()
    {
        if (string.IsNullOrWhiteSpace(added_role_name))
        {
            Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = "Не указано имя роли" }];
            return;
        }
        ResponseBaseModel rest = !string.IsNullOrEmpty(UserInfo?.Email)
        ? await IdentityRepo.AddRoleToUser(new() { Email = UserInfo.Email, RoleName = added_role_name })
        : await IdentityRepo.CreateNewRole(added_role_name.Trim());

        Messages = rest.Messages;
        if (!rest.Success())
            return;
        added_role_name = "";

        if (myGrid is not null)
            await myGrid.RefreshDataAsync();
    }
}