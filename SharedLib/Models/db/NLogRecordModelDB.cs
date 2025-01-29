////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// NLog record
/// </summary>
[Index(nameof(RecordTime)), Index(nameof(ApplicationName)), Index(nameof(ContextPrefix)), Index(nameof(RecordLevel)), Index(nameof(Logger))]
public class NLogRecordModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ApplicationName
    /// </summary>
    public required string ApplicationName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? ContextPrefix { get; set; }

    /// <summary>
    /// RecordTime
    /// </summary>
    public DateTime RecordTime { get; set; }

    /// <summary>
    /// RecordLevel
    /// </summary>
    public required string RecordLevel { get; set; }

    /// <inheritdoc/>
    public string? RecordMessage { get; set; }

    /// <inheritdoc/>
    public string? ExceptionMessage { get; set; }

    /// <inheritdoc/>
    public string? Logger { get; set; }

    /// <inheritdoc/>
    public string? CallSite { get; set; }

    /// <inheritdoc/>
    public string? StackTrace { get; set; }

    /// <inheritdoc/>
    public string? AllEventProperties { get; set; }
}