////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// GetIssuesForUserRequestModel
/// </summary>
public class GetIssuesForUserRequestModel : TPaginationRequestModel<UserCrossIdsModel>
{
    /// <summary>
    /// ProjectId
    /// </summary>
    public int ProjectId { get; set; }
}