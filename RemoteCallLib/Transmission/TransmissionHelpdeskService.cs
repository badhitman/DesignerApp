////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <inheritdoc/>
public class TransmissionHelpdeskService(IRabbitClient rabbitClient) : IHelpdeskRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> TelegramMessageIncoming(TelegramIncomingMessageModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.IncomingTelegramMessageHelpdeskReceive, req) ?? new();

    #region articles
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateRubricsForArticle(ArticleRubricsSetModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.RubricsForArticleSetReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<ArticleModelDB>> ArticlesSelect(TPaginationRequestModel<SelectArticlesRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<ArticleModelDB>>(GlobalStaticConstants.TransmissionQueues.ArticlesSelectHelpdeskReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> ArticleCreateOrUpdate(ArticleModelDB article)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.ArticleUpdateHelpdeskReceive, article) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<ArticleModelDB[]>> ArticlesRead(int[] req)
        => await rabbitClient.MqRemoteCall<TResponseModel<ArticleModelDB[]>>(GlobalStaticConstants.TransmissionQueues.ArticlesReadReceive, req) ?? new();
    #endregion

    #region rubric
    /// <inheritdoc/>
    public async Task<TResponseModel<List<RubricIssueHelpdeskModelDB>>> RubricsGet(IEnumerable<int> rubricsIds)
        => await rabbitClient.MqRemoteCall<TResponseModel<List<RubricIssueHelpdeskModelDB>>>(GlobalStaticConstants.TransmissionQueues.RubricsForIssuesGetHelpdeskReceive, rubricsIds) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<List<RubricIssueHelpdeskModelDB>>> RubricRead(int rubricId)
        => await rabbitClient.MqRemoteCall<TResponseModel<List<RubricIssueHelpdeskModelDB>>>(GlobalStaticConstants.TransmissionQueues.RubricForIssuesReadHelpdeskReceive, rubricId) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> RubricCreateOrUpdate(RubricIssueHelpdeskModelDB issueTheme)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.RubricForIssuesUpdateHelpdeskReceive, issueTheme) ?? new();

    /// <inheritdoc/>
    public async Task<List<UniversalBaseModel>> RubricsList(RubricsListRequestModel req)
        => await rabbitClient.MqRemoteCall<List<UniversalBaseModel>>(GlobalStaticConstants.TransmissionQueues.RubricsForIssuesListHelpdeskReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RubricMove(RowMoveModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.RubricForIssuesMoveHelpdeskReceive, req) ?? new();
    #endregion

    #region issue
    /// <inheritdoc/>
    public async Task<TResponseModel<int>> IssueCreateOrUpdate(TAuthRequestModel<UniversalUpdateRequestModel> issue)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.IssueUpdateHelpdeskReceive, issue) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>>> IssuesSelect(TAuthRequestModel<TPaginationRequestModel<SelectIssuesRequestModel>> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>>>(GlobalStaticConstants.TransmissionQueues.IssuesSelectHelpdeskReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB[]>> IssuesRead(TAuthRequestModel<IssuesReadRequestModel> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<IssueHelpdeskModelDB[]>>(GlobalStaticConstants.TransmissionQueues.IssuesGetHelpdeskReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> SubscribeUpdate(TAuthRequestModel<SubscribeUpdateRequestModel> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.SubscribeIssueUpdateHelpdeskReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<List<SubscriberIssueHelpdeskModelDB>> SubscribesList(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<List<SubscriberIssueHelpdeskModelDB>>(GlobalStaticConstants.TransmissionQueues.SubscribesIssueListHelpdeskReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ExecuterUpdate(TAuthRequestModel<UserIssueModel> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.ExecuterIssueUpdateHelpdeskReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> StatusChange(TAuthRequestModel<StatusChangeRequestModel> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.StatusChangeIssueHelpdeskReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> PulsePush(PulseRequestModel req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.PulseIssuePushHelpdeskReceive, req, waitResponse) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<PulseViewModel>>> PulseSelectJournal(TAuthRequestModel<TPaginationRequestModel<UserIssueModel>> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<TPaginationResponseModel<PulseViewModel>>>(GlobalStaticConstants.TransmissionQueues.PulseJournalHelpdeskSelectReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<IssueHelpdeskModel>> ConsoleIssuesSelect(TPaginationRequestModel<ConsoleIssuesRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<IssueHelpdeskModel>>(GlobalStaticConstants.TransmissionQueues.ConsoleIssuesSelectHelpdeskReceive, req) ?? new();
    #endregion

    #region message
    /// <inheritdoc/>
    public async Task<TResponseModel<int>> MessageCreateOrUpdate(TAuthRequestModel<IssueMessageHelpdeskBaseModel> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.MessageOfIssueUpdateHelpdeskReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> MessageVote(TAuthRequestModel<VoteIssueRequestModel> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.MessageOfIssueVoteHelpdeskReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueMessageHelpdeskModelDB[]>> MessagesList(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<IssueMessageHelpdeskModelDB[]>>(GlobalStaticConstants.TransmissionQueues.MessagesOfIssueListHelpdeskReceive, req) ?? new();
    #endregion
}