////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

public class TransmissionHelpdeskService(IRabbitClient rabbitClient) : IHelpdeskRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CreateIssue(IssueHelpdeskModelDB issue)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.CreateIssueHelpdeskReceive, issue);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CreateIssuesTheme(IssueThemeHelpdeskModelDB issueTheme)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.CreateIssuesThemeHelpdeskReceive, issueTheme);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB[]?>> GetIssuesForUser(TPaginationRequestModel<UserCrossIdsModel> user)
        => await rabbitClient.MqRemoteCall<IssueHelpdeskModelDB[]>(GlobalStaticConstants.TransmissionQueues.GetIssuesForUserHelpdeskReceive, user);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueThemeHelpdeskModelDB[]?>> GetThemesIssues()
        => await rabbitClient.MqRemoteCall<IssueThemeHelpdeskModelDB[]>(GlobalStaticConstants.TransmissionQueues.GetIssuesForUserHelpdeskReceive);
}