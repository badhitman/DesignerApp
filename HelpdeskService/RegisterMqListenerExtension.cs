////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;
using Transmission.Receives.helpdesk;

namespace HelpdeskService;

/// <summary>
/// MQ listen
/// </summary>
public static class RegisterMqListenerExtension
{
    /// <summary>
    /// RegisterMqListeners
    /// </summary>
    public static IServiceCollection HelpdeskRegisterMqListeners(this IServiceCollection services)
    {
        return services
            .RegisterMqListener<RubricsListReceive, RubricsListRequestModel?, UniversalBaseModel[]?>()
            .RegisterMqListener<RubricCreateOrUpdateReceive, RubricIssueHelpdeskModelDB?, int?>()
            .RegisterMqListener<IssuesSelectReceive, TPaginationRequestModel<SelectIssuesRequestModel>?, TPaginationResponseModel<IssueHelpdeskModel>?>()
            .RegisterMqListener<ArticlesSelectReceive, TPaginationRequestModel<SelectArticlesRequestModel>?, TPaginationResponseModel<ArticleModelDB>?>()
            .RegisterMqListener<IssueCreateOrUpdateReceive, TAuthRequestModel<UniversalUpdateRequestModel>?, int>()
            .RegisterMqListener<MessageVoteReceive, TAuthRequestModel<VoteIssueRequestModel>?, bool?>()
            .RegisterMqListener<MessageUpdateOrCreateReceive, TAuthRequestModel<IssueMessageHelpdeskBaseModel>?, int?>()
            .RegisterMqListener<RubricMoveReceive, RowMoveModel?, bool?>()
            .RegisterMqListener<SetWebConfigReceive, HelpdeskConfigModel?, object?>()
            .RegisterMqListener<UpdateRubricsForArticleReceive, ArticleRubricsSetModel?, bool?>()
            .RegisterMqListener<ArticlesReadReceive, int[]?, ArticleModelDB[]?>()
            .RegisterMqListener<ArticleCreateOrUpdateReceive, ArticleModelDB?, int?>()
            .RegisterMqListener<IssuesReadReceive, TAuthRequestModel<IssuesReadRequestModel>?, IssueHelpdeskModelDB[]?>()
            .RegisterMqListener<RubricReadReceive, int?, List<RubricIssueHelpdeskModelDB>?>()
            .RegisterMqListener<RubricsGetReceive, int[]?, List<RubricIssueHelpdeskModelDB>?>()
            .RegisterMqListener<SubscribeUpdateReceive, TAuthRequestModel<SubscribeUpdateRequestModel>?, bool?>()
            .RegisterMqListener<SubscribesListReceive, TAuthRequestModel<int>?, SubscriberIssueHelpdeskModelDB[]?>()
            .RegisterMqListener<ExecuterUpdateReceive, TAuthRequestModel<UserIssueModel>?, bool>()
            .RegisterMqListener<MessagesListReceive, TAuthRequestModel<int>?, IssueMessageHelpdeskModelDB[]>()
            .RegisterMqListener<StatusChangeReceive, TAuthRequestModel<StatusChangeRequestModel>?, bool>()
            .RegisterMqListener<PulseIssueReceive, PulseRequestModel?, bool>()
            .RegisterMqListener<PulseJournalReceive, TPaginationRequestModel<UserIssueModel>?, TPaginationResponseModel<PulseViewModel>>()
            .RegisterMqListener<TelegramMessageIncomingReceive, TelegramIncomingMessageModel?, bool>()
            .RegisterMqListener<ConsoleIssuesSelectReceive, TPaginationRequestModel<ConsoleIssuesRequestModel>?, TPaginationResponseModel<IssueHelpdeskModel>>()
            ;
    }
}