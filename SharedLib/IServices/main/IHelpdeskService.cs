////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Globalization;

namespace SharedLib;

/// <summary>
/// Helpdesk (service)
/// </summary>
public interface IHelpdeskService
{
    /// <summary>
    /// ReplaceTags
    /// </summary>
    public static string ReplaceTags(string documentName, DateTime dateCreated, int documentId, StatusesDocumentsEnum stepIssue, string raw, string? clearBaseUri, string aboutDocument, bool clearMd = false, string documentPagePath = "issue-card")
    {
        return raw.Replace(GlobalStaticConstants.DocumentNameProperty, documentName)
        .Replace(GlobalStaticConstants.DocumentDateProperty, $"{dateCreated.GetCustomTime().ToString("d", GlobalStaticConstants.RU)} {dateCreated.GetCustomTime().ToString("t", GlobalStaticConstants.RU)}")
        .Replace(GlobalStaticConstants.DocumentStatusProperty, stepIssue.DescriptionInfo())
        .Replace(GlobalStaticConstants.DocumentLinkAddressProperty, clearMd ? $"{clearBaseUri}/{documentPagePath}/{documentId}" : $"<a href='{clearBaseUri}/{documentPagePath}/{documentId}'>{aboutDocument}</a>")
        .Replace(GlobalStaticConstants.HostAddressProperty, clearMd ? clearBaseUri : $"<a href='{clearBaseUri}'>{clearBaseUri}</a>");
    }

    #region messages
    /// <summary>
    /// Сообщение в обращение
    /// </summary>
    public Task<TResponseModel<int?>> MessageUpdateOrCreate(TAuthRequestModel<IssueMessageHelpdeskBaseModel> req);

    /// <summary>
    /// Message vote
    /// </summary>
    public Task<TResponseModel<bool?>> MessageVote(TAuthRequestModel<VoteIssueRequestModel> req);

    /// <summary>
    /// MessagesList
    /// </summary>
    public Task<TResponseModel<IssueMessageHelpdeskModelDB[]>> MessagesList(TAuthRequestModel<int> req);
    #endregion

    #region issues
    /// <summary>
    /// SubscribesList
    /// </summary>
    public Task<TResponseModel<List<SubscriberIssueHelpdeskModelDB>>> SubscribesList(TAuthRequestModel<int> req);

    /// <summary>
    /// IssuesSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>>> IssuesSelect(TAuthRequestModel<TPaginationRequestModel<SelectIssuesRequestModel>> req);

    /// <summary>
    /// ConsoleIssuesSelect
    /// </summary>
    public Task<TPaginationResponseModel<IssueHelpdeskModel>> ConsoleIssuesSelect(TPaginationRequestModel<ConsoleIssuesRequestModel> req);

    /// <summary>
    /// Subscribe update - of context user
    /// </summary>
    public Task<TResponseModel<bool>> ExecuterUpdate(TAuthRequestModel<UserIssueModel> req);

    /// <summary>
    /// Create (or update) Issue: Рубрика, тема и описание
    /// </summary>
    public Task<TResponseModel<int>> IssueCreateOrUpdate(TAuthRequestModel<UniversalUpdateRequestModel> req);

    /// <summary>
    /// Subscribe update - of context user
    /// </summary>
    public Task<TResponseModel<bool?>> SubscribeUpdate(TAuthRequestModel<SubscribeUpdateRequestModel> req);

    /// <summary>
    /// Status change
    /// </summary>
    public Task<TResponseModel<bool>> IssueStatusChange(TAuthRequestModel<StatusChangeRequestModel> req);

    /// <summary>
    /// Read issue - of context user
    /// </summary>
    public Task<TResponseModel<IssueHelpdeskModelDB[]>> IssuesRead(TAuthRequestModel<IssuesReadRequestModel> req);
    #endregion

    #region rubric
    /// <summary>
    /// Получить рубрики, вложенные в рубрику (если не указано, то root перечень)
    /// </summary>
    public Task<List<UniversalBaseModel>> RubricsList(RubricsListRequestModel req);

    /// <summary>
    /// RubricMove
    /// </summary>
    public Task<ResponseBaseModel> RubricMove(TAuthRequestModel<RowMoveModel> req);

    /// <summary>
    /// Rubric create (or update)
    /// </summary>
    public Task<TResponseModel<int>> RubricCreateOrUpdate(RubricIssueHelpdeskModelDB req);

    /// <summary>
    /// Rubric read
    /// </summary>
    public Task<TResponseModel<List<RubricIssueHelpdeskModelDB>>> RubricRead(int rubricId);

    /// <summary>
    /// Rubrics get
    /// </summary>
    public Task<TResponseModel<List<RubricIssueHelpdeskModelDB>>> RubricsGet(int[] rubricsIds);
    #endregion

    #region pulse
    /// <summary>
    /// PulseJournalSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<PulseViewModel>>> PulseJournalSelect(TAuthRequestModel<TPaginationRequestModel<UserIssueModel>> req);

    /// <summary>
    /// Регистрация события из обращения (логи).
    /// </summary>
    /// <remarks>
    /// Плюс рассылка уведомлений участникам события.
    /// </remarks>
    public Task<TResponseModel<bool>> PulsePush(PulseRequestModel req);

    #endregion

    /// <summary>
    /// Обработка входящего Telegram сообщения
    /// </summary>
    /// <remarks>
    /// Если это ответ в контексте заявки, тогда оно регистрируется и переправляется
    /// </remarks>
    public Task<ResponseBaseModel> TelegramMessageIncoming(TelegramIncomingMessageModel req);

    /// <summary>
    /// SetWebConfig
    /// </summary>
    public Task<ResponseBaseModel> SetWebConfig(HelpdeskConfigModel req);

    /// <summary>
    /// Очистить кеш сегмента консоли
    /// </summary>
    public Task ConsoleSegmentCacheEmpty(StatusesDocumentsEnum? Status = null);
}
