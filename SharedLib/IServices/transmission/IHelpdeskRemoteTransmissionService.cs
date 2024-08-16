////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Удалённый вызов команд в Helpdesk службе
/// </summary>
public interface IHelpdeskRemoteTransmissionService
{
    #region rubric
    /// <summary>
    /// Получить темы обращений
    /// </summary>
    public Task<TResponseModel<List<RubricIssueHelpdeskLowModel>?>> RubricsList(TProjectedRequestModel<int> req);

    /// <summary>
    /// Создать тему для обращений
    /// </summary>
    public Task<TResponseModel<int?>> RubricCreateOrUpdate(RubricIssueHelpdeskModelDB issueTheme);

    /// <summary>
    /// Сдвинуть рубрику
    /// </summary>
    public Task<TResponseModel<bool?>> RubricMove(RowMoveModel req);

    /// <summary>
    /// Прочитать данные рубрики (вместе со всеми вышестоящими родлителями)
    /// </summary>
    public Task<TResponseModel<List<RubricIssueHelpdeskModelDB>?>> RubricRead(int rubricId);
    #endregion

    #region issue
    /// <summary>
    /// Получить обращения для пользователя
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>?>> IssuesSelect(TPaginationRequestModel<GetIssuesForUserRequestModel> req);

    /// <summary>
    /// Создать обращение
    /// </summary>
    public Task<TResponseModel<int>> IssueCreateOrUpdate(TAuthRequestModel<IssueUpdateRequestModel> issue);

    /// <summary>
    /// Прочитать данные обращения
    /// </summary>
    public Task<TResponseModel<IssueHelpdeskModelDB?>> IssueRead(TAuthRequestModel<IssueReadRequestModel> req);

    /// <summary>
    /// Подписка на события в обращении (или отписка от событий)
    /// </summary>
    public Task<TResponseModel<bool?>> SubscribeUpdate(TAuthRequestModel<SubscribeUpdateRequestModel> req);

    /// <summary>
    /// Запрос подписчиков на обращение
    /// </summary>
    public Task<TResponseModel<SubscriberIssueHelpdeskModelDB[]?>> SubscribesList(TAuthRequestModel<int> req);

    /// <summary>
    /// Исполнитель обращения
    /// </summary>
    public Task<TResponseModel<bool>> ExecuterUpdate(TAuthRequestModel<UserUpdateRequestModel> req);
    #endregion

    #region message
    /// <summary>
    /// Сообщение из обращения помечается как ответ (либо этот признак снимается: в зависимости от запроса)
    /// </summary>
    public Task<TResponseModel<bool>> MessageVote(TAuthRequestModel<VoteIssueRequestModel> req);

    /// <summary>
    /// Добавить сообщение к обращению
    /// </summary>
    public Task<TResponseModel<int>> MessageCreateOrUpdate(TAuthRequestModel<IssueMessageHelpdeskBaseModel> req);

    /// <summary>
    /// Сообщения из обращения
    /// </summary>
    public Task<TResponseModel<IssueMessageHelpdeskModelDB[]?>> MessagesList(TAuthRequestModel<int> req);

    #endregion
}