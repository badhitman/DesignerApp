﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// CommandsAsEntries
/// </summary>
public class CommandAsEntryModel
{
    /// <summary>
    /// CommandName
    /// </summary>
    public required string CommandName { get; set; }

    /// <summary>
    /// Options
    /// </summary>
    public required IEnumerable<string> Options { get; set; }
}