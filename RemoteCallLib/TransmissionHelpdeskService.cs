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
    public async Task<TResponseModel<int>> CreateOrUpdateRubricIssues(RubricIssueHelpdeskModelDB issueTheme)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.CreateIssuesThemeHelpdeskReceive, issueTheme);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB[]?>> GetIssuesForUser(GetIssuesForUserRequestModel req)
        => await rabbitClient.MqRemoteCall<IssueHelpdeskModelDB[]>(GlobalStaticConstants.TransmissionQueues.GetIssuesForUserHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<RubricIssueHelpdeskModelDB[]?>> GetRubricsIssues(ProjectOwnedRequestModel req)
        => await rabbitClient.MqRemoteCall<RubricIssueHelpdeskModelDB[]>(GlobalStaticConstants.TransmissionQueues.GetRubricsIssuesHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> SetMessageIssueAsResponse(SetMessageAsResponseIssueRequestModel req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.SetMessageAsResponseIssueHelpdeskReceive, req);

    public async Task<TResponseModel<bool>> UpdateMessageIssue(UpdateMessageRequestModel req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.UpdateMessageOfIssueHelpdeskReceive, req);
}