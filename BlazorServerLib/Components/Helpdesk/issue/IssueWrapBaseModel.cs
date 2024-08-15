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

    /// <summary>
    /// CurrentUser
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required IssueHelpdeskModelDB Issue { get; set; }

    /// <summary>
    /// UsersIdentityDump
    /// </summary>
    [CascadingParameter]
    public UserInfoModel[]? UsersIdentityDump { get; set; }
}