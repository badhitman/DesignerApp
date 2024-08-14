////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Удалённый вызов команд в Helpdesk службе
/// </summary>
public interface IHelpdeskRemoteTransmissionService
{
    /// <summary>
    /// Получить темы обращений
    /// </summary>
    public Task<TResponseModel<List<RubricIssueHelpdeskLowModel>?>> RubricsList(ProjectOwnedRequestModel req);

    /// <summary>
    /// Создать тему для обращений
    /// </summary>
    public Task<TResponseModel<int?>> RubricCreateOrUpdate(RubricIssueHelpdeskModelDB issueTheme);

    /// <summary>
    /// Сдвинуть рубрику
    /// </summary>
    public Task<TResponseModel<bool?>> RubricMove(RowMoveModel req);



    /// <summary>
    /// Получить обращения для пользователя
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>?>> IssuesSelect(TPaginationRequestModel<GetIssuesForUserRequestModel> req);

    /// <summary>
    /// Создать обращение
    /// </summary>
    public Task<TResponseModel<int>> IssueCreateOrUpdate(IssueUpdateRequest issue);

    /// <summary>
    /// Прочитать данные обращения
    /// </summary>
    public Task<TResponseModel<IssueHelpdeskModelDB?>> IssueRead(IssueReadRequestModel req);



    /// <summary>
    /// Сообщение из обращения помечается как ответ (либо этот признак снимается: в зависимости от запроса)
    /// </summary>
    public Task<TResponseModel<bool>> MessageVote(VoteIssueRequestModel req);

    /// <summary>
    /// Добавить сообщение к обращению
    /// </summary>
    public Task<TResponseModel<int>> MessageCreateOrUpdate(IssueMessageHelpdeskBaseModel req);
}