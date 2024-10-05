////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <inheritdoc/>
public class TransmissionHelpdeskService(IRabbitClient rabbitClient) : IHelpdeskRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> TelegramMessageIncoming(TelegramIncomingMessageModel req)
        => await rabbitClient.MqRemoteCall<bool?>(GlobalStaticConstants.TransmissionQueues.IncomingTelegramMessageHelpdeskReceive, req);

    #region rubric
    /// <inheritdoc/>
    public async Task<TResponseModel<List<RubricIssueHelpdeskModelDB>?>> RubricRead(int rubricId)
        => await rabbitClient.MqRemoteCall<List<RubricIssueHelpdeskModelDB>?>(GlobalStaticConstants.TransmissionQueues.RubricForIssuesReadHelpdeskReceive, rubricId);

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> RubricCreateOrUpdate(RubricIssueHelpdeskModelDB issueTheme)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.RubricForIssuesUpdateHelpdeskReceive, issueTheme);

    /// <inheritdoc/>
    public async Task<TResponseModel<List<RubricIssueHelpdeskLowModel>>> RubricsList(RubricsListRequestModel req)
        => await rabbitClient.MqRemoteCall<List<RubricIssueHelpdeskLowModel>>(GlobalStaticConstants.TransmissionQueues.RubricsForIssuesListHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> RubricMove(RowMoveModel req)
        => await rabbitClient.MqRemoteCall<bool?>(GlobalStaticConstants.TransmissionQueues.RubricForIssuesMoveHelpdeskReceive, req);
    #endregion

    #region issue
    /// <inheritdoc/>
    public async Task<TResponseModel<int>> IssueCreateOrUpdate(TAuthRequestModel<IssueUpdateRequestModel> issue)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.IssueUpdateHelpdeskReceive, issue);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>>> IssuesSelect(TPaginationRequestModel<GetIssuesForUserRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<IssueHelpdeskModel>>(GlobalStaticConstants.TransmissionQueues.IssuesSelectHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB[]>> IssuesRead(TAuthRequestModel<IssuesReadRequestModel> req)
        => await rabbitClient.MqRemoteCall<IssueHelpdeskModelDB[]>(GlobalStaticConstants.TransmissionQueues.IssuesGetHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> SubscribeUpdate(TAuthRequestModel<SubscribeUpdateRequestModel> req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.SubscribeIssueUpdateHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<SubscriberIssueHelpdeskModelDB[]?>> SubscribesList(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<SubscriberIssueHelpdeskModelDB[]?>(GlobalStaticConstants.TransmissionQueues.SubscribesIssueListHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ExecuterUpdate(TAuthRequestModel<UserIssueModel> req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.ExecuterIssueUpdateHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> StatusChange(TAuthRequestModel<StatusChangeRequestModel> req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.StatusChangeIssueHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> PulsePush(PulseRequestModel req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.PulseIssuePushHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<PulseViewModel>>> PulseJournal(TPaginationRequestModel<UserIssueModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<PulseViewModel>>(GlobalStaticConstants.TransmissionQueues.PulseJournalHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>>> ConsoleIssuesSelect(TPaginationRequestModel<ConsoleIssuesRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<IssueHelpdeskModel>>(GlobalStaticConstants.TransmissionQueues.ConsoleIssuesSelectHelpdeskReceive, req);
    #endregion

    #region message
    /// <inheritdoc/>
    public async Task<TResponseModel<int>> MessageCreateOrUpdate(TAuthRequestModel<IssueMessageHelpdeskBaseModel> req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.MessageOfIssueUpdateHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> MessageVote(TAuthRequestModel<VoteIssueRequestModel> req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.MessageOfIssueVoteHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueMessageHelpdeskModelDB[]>> MessagesList(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<IssueMessageHelpdeskModelDB[]>(GlobalStaticConstants.TransmissionQueues.MessagesOfIssueListHelpdeskReceive, req);
    #endregion
}