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
            .RegisterMqListener<RubricsListReceive, RubricsListRequestModel, List<UniversalBaseModel>>()
            .RegisterMqListener<RubricCreateOrUpdateReceive, RubricIssueHelpdeskModelDB, TResponseModel<int>>()
            .RegisterMqListener<IssuesSelectReceive, TAuthRequestModel<TPaginationRequestModel<SelectIssuesRequestModel>>, TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>>>()
            .RegisterMqListener<ArticlesSelectReceive, TPaginationRequestModel<SelectArticlesRequestModel>, TPaginationResponseModel<ArticleModelDB>>()
            .RegisterMqListener<IssueCreateOrUpdateReceive, TAuthRequestModel<UniversalUpdateRequestModel>, TResponseModel<int>>()
            .RegisterMqListener<MessageVoteReceive, TAuthRequestModel<VoteIssueRequestModel>, TResponseModel<bool?>>()
            .RegisterMqListener<MessageUpdateOrCreateReceive, TAuthRequestModel<IssueMessageHelpdeskBaseModel>, TResponseModel<int?>>()
            .RegisterMqListener<RubricMoveReceive, RowMoveModel, TResponseModel<bool>>()
            .RegisterMqListener<SetWebConfigReceive, HelpdeskConfigModel, ResponseBaseModel>()
            .RegisterMqListener<UpdateRubricsForArticleReceive, ArticleRubricsSetModel, ResponseBaseModel>()
            .RegisterMqListener<ArticlesReadReceive, int[], TResponseModel<ArticleModelDB[]>>()
            .RegisterMqListener<ArticleCreateOrUpdateReceive, ArticleModelDB, TResponseModel<int>>()
            .RegisterMqListener<IssuesReadReceive, TAuthRequestModel<IssuesReadRequestModel>, TResponseModel<IssueHelpdeskModelDB[]>>()
            .RegisterMqListener<RubricReadReceive, int, TResponseModel<List<RubricIssueHelpdeskModelDB>>>()
            .RegisterMqListener<RubricsGetReceive, int[], TResponseModel<List<RubricIssueHelpdeskModelDB>>>()
            .RegisterMqListener<SubscribeUpdateReceive, TAuthRequestModel<SubscribeUpdateRequestModel>, TResponseModel<bool?>>()
            .RegisterMqListener<SubscribesListReceive, TAuthRequestModel<int>, List<SubscriberIssueHelpdeskModelDB>>()
            .RegisterMqListener<ExecuterUpdateReceive, TAuthRequestModel<UserIssueModel>, TResponseModel<bool>>()
            .RegisterMqListener<MessagesListReceive, TAuthRequestModel<int>, TResponseModel<IssueMessageHelpdeskModelDB[]>>()
            .RegisterMqListener<StatusChangeReceive, TAuthRequestModel<StatusChangeRequestModel>, TResponseModel<bool>>()
            .RegisterMqListener<PulseIssueReceive, PulseRequestModel, TResponseModel<bool>>()
            .RegisterMqListener<PulseJournalSelectReceive, TAuthRequestModel<TPaginationRequestModel<UserIssueModel>>, TResponseModel<TPaginationResponseModel<PulseViewModel>>>()
            .RegisterMqListener<TelegramMessageIncomingReceive, TelegramIncomingMessageModel, TResponseModel<bool>>()
            .RegisterMqListener<ConsoleIssuesSelectReceive, TPaginationRequestModel<ConsoleIssuesRequestModel>, TPaginationResponseModel<IssueHelpdeskModel>>()
            ;
    }
}