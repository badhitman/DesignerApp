////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

public class TransmissionHelpdeskService(IRabbitClient rabbitClient) : IHelpdeskRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<int>> AddNewMessageIntoIssue(IssueMessageHelpdeskBaseModel req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.AddNewMessageIntoIssueHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CreateOrUpdateIssue(IssueHelpdeskModelDB issue)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.CreateIssueHelpdeskReceive, issue);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CreateOrUpdateIssuesTheme(IssueThemeHelpdeskModelDB issueTheme)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.CreateIssuesThemeHelpdeskReceive, issueTheme);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB[]?>> GetIssuesForUser(TPaginationRequestModel<UserCrossIdsModel> user)
        => await rabbitClient.MqRemoteCall<IssueHelpdeskModelDB[]>(GlobalStaticConstants.TransmissionQueues.GetIssuesForUserHelpdeskReceive, user);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueThemeHelpdeskModelDB[]?>> GetThemesIssues()
        => await rabbitClient.MqRemoteCall<IssueThemeHelpdeskModelDB[]>(GlobalStaticConstants.TransmissionQueues.GetIssuesForUserHelpdeskReceive);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> SetMessageIssueAsResponse(SetMessageAsResponseIssueRequestModel req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.SetMessageAsResponseIssueHelpdeskReceive, req);

    public async Task<TResponseModel<bool>> UpdateMessageIssue(UpdateMessageRequestModel req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.UpdateMessageOfIssueHelpdeskReceive, req);
}