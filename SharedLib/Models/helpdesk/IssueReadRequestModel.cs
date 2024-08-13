////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Issue read request
/// </summary>
public class IssueReadRequestModel
{
    /// <summary>
    /// User id (Identity)
    /// </summary>
    public required string UserIdentityId { get; set; }

    /// <summary>
    /// IssueId
    /// </summary>
    public required int IssueId { get; set; }
}