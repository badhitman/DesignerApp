////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// MarkAsResponseModelDB
/// </summary>
public class MarkAsResponseModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ParentMessage
    /// </summary>
    public IssueMessageModelDB? Message { get; set; }
    /// <summary>
    /// ParentMessage
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public IssueModelDB? Issue { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public int IssueId { get; set; }
}