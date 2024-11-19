////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Helpdesk (service)
/// </summary>
public interface IHelpdeskService
{
    #region issues
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
    public Task<TResponseModel<int>> IssueCreateOrUpdate(TAuthRequestModel<IssueUpdateRequestModel> req);

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
    /// Rubric create (or update)
    /// </summary>
    public Task<TResponseModel<int>> RubricCreateOrUpdate(RubricIssueHelpdeskModelDB req);

    /// <summary>
    /// Rubric read
    /// </summary>
    public Task<TResponseModel<List<RubricIssueHelpdeskModelDB>?>> RubricRead(int rubricId);

    /// <summary>
    /// Rubrics get
    /// </summary>
    public Task<TResponseModel<List<RubricIssueHelpdeskModelDB>>> RubricsGet(int[] rubricsIds);
    #endregion

    /// <summary>
    /// Регистрация события из обращения (логи).
    /// </summary>
    /// <remarks>
    /// Плюс рассылка уведомлений участникам события.
    /// </remarks>
    public Task<TResponseModel<bool>> PulsePush(PulseRequestModel req);

    /// <summary>
    /// Сообщение в обращение
    /// </summary>
    public Task<TResponseModel<int?>> MessageUpdateOrCreate(TAuthRequestModel<IssueMessageHelpdeskBaseModel> req);

    /// <summary>
    /// Message vote
    /// </summary>
    public Task<TResponseModel<bool?>> MessageVote(TAuthRequestModel<VoteIssueRequestModel> req);

    /// <summary>
    /// Очистить кеш сегмента консоли
    /// </summary>
    public Task ConsoleSegmentCacheEmpty(StatusesDocumentsEnum? Status = null);
}
