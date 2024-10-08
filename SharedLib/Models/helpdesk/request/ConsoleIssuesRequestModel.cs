﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ConsoleIssuesRequestModel
/// </summary>
public class ConsoleIssuesRequestModel : SimpleBaseRequestModel
{
    /// <summary>
    /// FilterUserId
    /// </summary>
    public string? FilterUserId { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public required HelpdeskIssueStepsEnum Status { get; set; }
}