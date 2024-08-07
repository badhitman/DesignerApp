////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// MarkAsResponseModelDB
/// </summary>
public class MarkAsResponseHelpdeskModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ParentMessage
    /// </summary>
    public IssueMessageHelpdeskModelDB? Message { get; set; }
    /// <summary>
    /// ParentMessage
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public IssueHelpdeskModelDB? Issue { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public int IssueId { get; set; }
}