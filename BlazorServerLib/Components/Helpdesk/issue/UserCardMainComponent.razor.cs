////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// UserCardMainComponent
/// </summary>
public partial class UserCardMainComponent : IssueWrapBaseModel
{
    /// <summary>
    /// CloseAction
    /// </summary>
    [Parameter, EditorRequired]
    public required Action CloseAction { get; set; }

    /// <summary>
    /// Author
    /// </summary>
    [Parameter, EditorRequired]
    public required UserInfoModel Author { get; set; }
}