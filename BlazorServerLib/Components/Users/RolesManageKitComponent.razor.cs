////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Users;

/// <summary>
/// RolesManageKitComponent
/// </summary>
public partial class RolesManageKitComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IWebRemoteTransmissionService WebRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// RolesManageKit
    /// </summary>
    [Parameter, EditorRequired]
    public required IEnumerable<string> RolesManageKit { get; set; }

    /// <summary>
    /// User
    /// </summary>
    [Parameter, EditorRequired]
    public required UserInfoModel User { get; set; }


    bool IsShow { get; set; }

    static string RoleDescription(string name)
    {
        return name switch
        {
            GlobalStaticConstants.Roles.HelpDeskTelegramBotUnit => "Исполнитель",
            GlobalStaticConstants.Roles.HelpDeskTelegramBotManager => "Менеджер",
            GlobalStaticConstants.Roles.HelpDeskTelegramBotChatsManage => "Чаты",
            GlobalStaticConstants.Roles.HelpDeskTelegramBotRubricsManage => "Рубрики",
            _ => name
        };
    }

    bool IsChecked(string role_name)
    {
        return User.Roles?.Contains(role_name) == true;
    }

    async Task ChangeUserRole(ChangeEventArgs e, string roleName)
    {
        bool value_bool = e.Value is not null && (bool)e.Value == true;
        User.Roles ??= [];

        SetRoleFoeUserRequestModel req = new()
        {
            RoleName = roleName,
            UserIdentityId = User.UserId,
            Command = value_bool,
        };

        async Task Act()
        {
            TResponseModel<string[]?> res = await WebRepo.SetRoleForUser(req);
            SnackbarRepo.ShowMessagesResponse(res.Messages);
            if (!res.Success() || res.Response is null)
                return;

            User.Roles.Clear();
            User.Roles.AddRange(res.Response);
        };

        IsBusyProgress = true;
        if (value_bool && !User.Roles.Contains(roleName))
            await Act();
        else if (!value_bool && User.Roles.Contains(roleName))
        {
            req.Command = false;
            await Act();
        }
        IsBusyProgress = false;
    }
}