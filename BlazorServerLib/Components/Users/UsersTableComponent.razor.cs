////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components;
using SharedLib;
using System.Net.Mail;

namespace BlazorWebLib.Components.Users;

/// <summary>
/// UsersTableComponent
/// </summary>
public partial class UsersTableComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// UsersManageRepo
    /// </summary>
    [Inject]
    IUsersProfilesService UsersManageRepo { get; set; } = default!;


    /// <summary>
    /// OwnerRoleId
    /// </summary>
    [Parameter]
    public string? OwnerRoleId { get; set; }

    /// <summary>
    /// Скрыть колонку ролей
    /// </summary>
    [Parameter]
    public bool HideRolesColumn { get; set; }

    /// <summary>
    /// Скрыть колонку Claims
    /// </summary>
    [Parameter]
    public bool HideClaimsColumn { get; set; }

    /// <summary>
    /// RolesManageKit
    /// </summary>
    [Parameter]
    public string[]? RolesManageKit {  get; set; }

    PaginationState pagination = new() { ItemsPerPage = 15 };
    string nameFilter = string.Empty;
    QuickGrid<UserInfoModel>? myGrid;
    int numResults;
    GridItemsProvider<UserInfoModel>? foodRecallProvider;
    string? added_user_email;
    IEnumerable<ResultMessage>? Messages;
    RoleInfoModel? RoleInfo;

    static string LinkCssClass(UserInfoModel user)
    {
        return user.LockoutEnabled && user.LockoutEnd.GetValueOrDefault(DateTimeOffset.MinValue) > DateTimeOffset.UtcNow ? "text-decoration-line-through" : "";
    }

    async Task SetUserLock(string userId, bool locked_set)
    {
        ResponseBaseModel rest = await UsersManageRepo.SetLockUser(userId, locked_set);
        Messages = rest.Messages;
        if (myGrid is not null)
            await myGrid.RefreshDataAsync();
    }

    async Task DeleteUser(string user_email)
    {
        if (string.IsNullOrEmpty(RoleInfo?.Name))
        {
            Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = "пустое имя роли. error {062C0753-24D3-4F84-A635-514832D978D0}" }];
            return;
        }

        if (!MailAddress.TryCreate(user_email, out _))
        {
            Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = "исключение пользователя без email" }];
            return;
        }

        ResponseBaseModel rest = await UsersManageRepo.DeleteRoleFromUser(RoleInfo.Name, user_email);

        Messages = rest.Messages;
        if (!rest.Success())
            return;
        added_user_email = "";

        if (myGrid is not null)
            await myGrid.RefreshDataAsync();
    }

    async Task AddRoleToUser()
    {
        if (!MailAddress.TryCreate(added_user_email, out _))
        {
            Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = "email пользователя не корректный" }];
            return;
        }
        if (string.IsNullOrWhiteSpace(RoleInfo?.Name))
        {
            Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = "role name IsNullOrWhiteSpace. error {2DFAFA92-B8E8-4914-832B-1DE267D39A18}" }];
            return;
        }

        ResponseBaseModel rest = await UsersManageRepo.AddRoleToUser(RoleInfo.Name, added_user_email);
        Messages = rest.Messages;
        if (!rest.Success())
            return;
        added_user_email = "";

        if (myGrid is not null)
            await myGrid.RefreshDataAsync();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrWhiteSpace(OwnerRoleId))
        {
            TResponseModel<RoleInfoModel?> rest = await UsersManageRepo.GetRole(OwnerRoleId);
            Messages = rest.Messages;
            if (!rest.Success())
                return;
            RoleInfo = rest.Response;
        }

        foodRecallProvider = async req =>
        {
            TPaginationStrictResponseModel<UserInfoModel> res = await UsersManageRepo.FindUsersAsync(new FindWithOwnedRequestModel()
            {
                FindQuery = nameFilter,
                PageNum = pagination.CurrentPageIndex,
                PageSize = pagination.ItemsPerPage,
                OwnerId = RoleInfo?.Id
            });

            if (res.Response is null)
            {
                string msg = "UsersInfo is null. error {B57C9FB4-FC33-44A1-90C7-E52C00A041EE}";
                throw new Exception(msg);
            }

            if (numResults != res.TotalRowsCount)
            {
                numResults = res.TotalRowsCount;
                StateHasChanged();
            }

            return GridItemsProviderResult.From<UserInfoModel>(res.Response, res.TotalRowsCount);
        };
    }
}