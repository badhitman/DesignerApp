////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// IssueWrapBaseModel
/// </summary>
public abstract class IssueWrapBaseModel : BlazorBusyComponentBaseModel
{
    [Inject]
    internal ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    internal IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    internal NavigationManager NavRepo { get; set; } = default!;


    /// <summary>
    /// CurrentUser
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required UserInfoMainModel CurrentUser { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required IssueHelpdeskModelDB Issue { get; set; }

    /// <summary>
    /// can edit: is admin || executor || author || admin || helpdesk-manager
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required bool CanEdit { get; set; }

    /// <summary>
    /// UsersIdentityDump
    /// </summary>
    [CascadingParameter]
    public List<UserInfoModel> UsersIdentityDump { get; set; } = [];
}