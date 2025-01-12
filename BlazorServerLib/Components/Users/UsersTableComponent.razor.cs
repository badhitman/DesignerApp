////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Mail;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Users;

/// <summary>
/// UsersTableComponent
/// </summary>
public partial class UsersTableComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IStorageTransmission SerializeStorageRepo { get; set; } = default!;

    /// <summary>
    /// JS
    /// </summary>
    [Inject]
    protected IJSRuntime JS { get; set; } = default!;

    /// <summary>
    /// UsersManageRepo
    /// </summary>
    [Inject]
    IUsersProfilesService UsersManageRepo { get; set; } = default!;

    [Inject]
    IIdentityTransmission IdentityRepo { get; set; } = default!;

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
    public IEnumerable<string>? RolesManageKit { get; set; }

    /// <summary>
    /// RolesMarkers
    /// </summary>
    [Parameter]
    public EntryAltModel[]? RolesMarkers { get; set; }


    private readonly PaginationState pagination = new() { ItemsPerPage = 15 };

    QuickGrid<UserInfoModel>? myGrid;
    int numResults;
    GridItemsProvider<UserInfoModel>? foodRecallProvider;
    string? added_user_email;
    IEnumerable<ResultMessage>? Messages;
    RoleInfoModel? RoleInfo;
    private readonly Stack<string> RowsStack = [];
    bool showClaimsUsers;

    /// <inheritdoc/>
    protected override void OnAfterRender(bool firstRender)
    {
        if (HideClaimsColumn || !showClaimsUsers)
            return;

        lock (RowsStack)
        {
            if (RowsStack.Count == 0)
                return;

            string row = RowsStack.Pop();
            Task.Run(async () => { await JS.InvokeAsync<int>("FrameHeightUpdate.Reload", row); });
        }
    }

    void PushRowGuid(string rowGuid)
    {
        lock (RowsStack)
        {
            if (RowsStack.Contains(rowGuid))
                return;
            RowsStack.Push(rowGuid);
        }
    }

    //string? PopRowGuid()
    //{
    //    lock (RowsStack)
    //    {
    //        if (RowsStack.Count == 0)
    //            return null;
    //        return RowsStack.Pop();
    //    }
    //}

    static string LinkCssClass(UserInfoModel user)
    {
        return user.LockoutEnabled && user.LockoutEnd.GetValueOrDefault(DateTimeOffset.MinValue) > DateTimeOffset.UtcNow ? "text-decoration-line-through" : "";
    }

    async Task SetUserLock(string userId, bool locked_set)
    {
        ResponseBaseModel rest = await UsersManageRepo.SetLockUser(new() { Set = locked_set, UserId = userId });
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

        ResponseBaseModel rest = await IdentityRepo.DeleteRoleFromUser(new() { Email = user_email, RoleName = RoleInfo.Name });

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

        ResponseBaseModel rest = await IdentityRepo.AddRoleToUser(new() { Email = added_user_email, RoleName = RoleInfo.Name });
        Messages = rest.Messages;
        if (!rest.Success())
            return;
        added_user_email = "";

        if (myGrid is not null)
            await myGrid.RefreshDataAsync();
    }

    static MarkupString ClaimsHtml(UserInfoModel ctx)
    {
        if (ctx.Claims is null)
            return (MarkupString)"<b>-не загружено-</b>";

        if (ctx.Claims.Length == 0)
            return (MarkupString)"<i>~ нет</i>";

        return (MarkupString)ctx.ClaimsAsString(";");
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await SetBusy();
        TResponseModel<bool?> res_ShowClaimsUsers = await SerializeStorageRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ShowClaimsUser);
        showClaimsUsers = res_ShowClaimsUsers.Response == true;

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
                FindQuery = string.Empty,
                PageNum = pagination.CurrentPageIndex,
                PageSize = pagination.ItemsPerPage,
                OwnerId = RoleInfo?.Id
            });

            if (res.Response is null)
            {
                string msg = "UsersInfo is null. error {B57C9FB4-FC33-44A1-90C7-E52C00A041EE}";
                throw new Exception(msg);
            }
            await SetBusy(false);
            if (numResults != res.TotalRowsCount)
            {
                numResults = res.TotalRowsCount;
                StateHasChanged();
            }

            return GridItemsProviderResult.From(res.Response, res.TotalRowsCount);
        };
    }
}