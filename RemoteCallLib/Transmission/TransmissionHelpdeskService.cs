////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <inheritdoc/>
public class TransmissionHelpdeskService(IRabbitClient rabbitClient) : IHelpdeskRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> RubricForIssuesCreateOrUpdate(RubricIssueHelpdeskModelDB issueTheme)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.RubricForIssuesUpdateHelpdeskReceive, issueTheme);

    /// <inheritdoc/>
    public async Task<TResponseModel<List<RubricIssueHelpdeskLowModel>?>> RubricsForIssuesList(ProjectOwnedRequestModel req)
        => await rabbitClient.MqRemoteCall<List<RubricIssueHelpdeskLowModel>>(GlobalStaticConstants.TransmissionQueues.RubricsForIssuesListHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> RubricForIssuesMove(RowMoveModel req)
        => await rabbitClient.MqRemoteCall<bool?>(GlobalStaticConstants.TransmissionQueues.RubricForIssuesMoveHelpdeskReceive, req);



    /// <inheritdoc/>
    public async Task<TResponseModel<int>> IssueCreateOrUpdate(IssueHelpdeskModelDB issue)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.IssueUpdateHelpdeskReceive, issue);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>?>> IssuesSelect(TPaginationRequestModel<GetIssuesForUserRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<IssueHelpdeskModel>>(GlobalStaticConstants.TransmissionQueues.IssuesSelectHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB?>> IssueRead(IssueReadRequestModel req)
        => await rabbitClient.MqRemoteCall<IssueHelpdeskModelDB>(GlobalStaticConstants.TransmissionQueues.IssueGetHelpdeskReceive, req);



    /// <inheritdoc/>
    public async Task<TResponseModel<int>> MessageForIssueCreateOrUpdate(IssueMessageHelpdeskBaseModel req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.MessageOfIssueUpdateHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> MessageOfIssueSetAsResponse(VoteIssueRequestModel req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.MessageOfIssueSetAsResponseHelpdeskReceive, req);
}