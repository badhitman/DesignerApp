////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <inheritdoc/>
public class TransmissionHelpdeskService(IRabbitClient rabbitClient) : IHelpdeskRemoteTransmissionService
{
    #region rubric
    /// <inheritdoc/>
    public async Task<TResponseModel<List<RubricIssueHelpdeskModelDB>?>> RubricRead(int rubricId)
        => await rabbitClient.MqRemoteCall<List<RubricIssueHelpdeskModelDB>?>(GlobalStaticConstants.TransmissionQueues.RubricForIssuesReadHelpdeskReceive, rubricId);

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> RubricCreateOrUpdate(RubricIssueHelpdeskModelDB issueTheme)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.RubricForIssuesUpdateHelpdeskReceive, issueTheme);

    /// <inheritdoc/>
    public async Task<TResponseModel<List<RubricIssueHelpdeskLowModel>?>> RubricsList(TProjectedRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<List<RubricIssueHelpdeskLowModel>>(GlobalStaticConstants.TransmissionQueues.RubricsForIssuesListHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> RubricMove(RowMoveModel req)
        => await rabbitClient.MqRemoteCall<bool?>(GlobalStaticConstants.TransmissionQueues.RubricForIssuesMoveHelpdeskReceive, req);
    #endregion

    #region issue
    /// <inheritdoc/>
    public async Task<TResponseModel<int>> IssueCreateOrUpdate(TAuthRequestModel<IssueUpdateRequest> issue)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.IssueUpdateHelpdeskReceive, issue);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>?>> IssuesSelect(TPaginationRequestModel<GetIssuesForUserRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<IssueHelpdeskModel>>(GlobalStaticConstants.TransmissionQueues.IssuesSelectHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB?>> IssueRead(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<IssueHelpdeskModelDB>(GlobalStaticConstants.TransmissionQueues.IssueGetHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> SubscribeUpdate(TAuthRequestModel<SubscribeUpdateRequestModel> req)
        => await rabbitClient.MqRemoteCall<bool?>(GlobalStaticConstants.TransmissionQueues.SubscribeIssueUpdateHelpdeskReceive, req);
    #endregion

    #region message
    /// <inheritdoc/>
    public async Task<TResponseModel<int>> MessageCreateOrUpdate(IssueMessageHelpdeskBaseModel req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.MessageOfIssueUpdateHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> MessageVote(VoteIssueRequestModel req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.MessageOfIssueSetAsResponseHelpdeskReceive, req);
    #endregion
}