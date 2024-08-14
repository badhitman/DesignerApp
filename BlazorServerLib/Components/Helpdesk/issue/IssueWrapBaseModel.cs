////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// IssueWrapBaseModel
/// </summary>
public class IssueWrapBaseModel : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Issue
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required IssueHelpdeskModelDB Issue { get; set; }
}