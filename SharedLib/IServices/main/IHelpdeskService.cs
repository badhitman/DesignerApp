////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Helpdesk (service)
/// </summary>
public interface IHelpdeskService
{
    /// <summary>
    /// ConsoleIssuesSelect
    /// </summary>
    public Task<TPaginationResponseModel<IssueHelpdeskModel>> ConsoleIssuesSelect(TPaginationRequestModel<ConsoleIssuesRequestModel> req);
}
