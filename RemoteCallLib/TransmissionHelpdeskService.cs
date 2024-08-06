////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

public class TransmissionHelpdeskService(IRabbitClient rabbitClient) : IHelpdeskRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CreateIssue(IssueModelDB issue)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.CreateIssueHelpdeskReceive, issue);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CreateIssuesTheme(IssueThemeModelDB issueTheme)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.CreateIssuesThemeHelpdeskReceive, issueTheme);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueModelDB[]?>> GetIssuesForUser(TPaginationRequestModel<(long? telegramId, string? identityId)> user)
        => await rabbitClient.MqRemoteCall<IssueModelDB[]>(GlobalStaticConstants.TransmissionQueues.GetIssuesForUserHelpdeskReceive, user);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueThemeModelDB[]?>> GetThemesIssues()
        => await rabbitClient.MqRemoteCall<IssueThemeModelDB[]>(GlobalStaticConstants.TransmissionQueues.GetIssuesForUserHelpdeskReceive);
}