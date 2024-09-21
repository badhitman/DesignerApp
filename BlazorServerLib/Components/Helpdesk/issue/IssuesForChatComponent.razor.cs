////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// Обращения/заявки для чата
/// </summary>
public partial class IssuesForChatComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ILogger<IssuesForChatComponent> LoggerRepo { get; set; } = default!;

    /// <summary>
    /// Issues
    /// </summary>
    [Parameter, EditorRequired]
    public required IssueHelpdeskModel[] Issues { get; set; }


    bool IsExpand;
}