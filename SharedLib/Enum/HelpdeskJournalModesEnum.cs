////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Helpdesk journal modes: All, ActualOnly, ArchiveOnly
/// </summary>
public enum HelpdeskJournalModesEnum
{
    /// <summary>
    /// All
    /// </summary>
    [Description("Все")]
    All,

    /// <summary>
    /// Actual only
    /// </summary>
    [Description("Актуальные")]
    ActualOnly,

    /// <summary>
    /// Archive only
    /// </summary>
    [Description("Архивные")]
    ArchiveOnly,
}