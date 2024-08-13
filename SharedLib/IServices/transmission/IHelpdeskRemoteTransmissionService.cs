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
    public Task<TResponseModel<List<RubricIssueHelpdeskLowModel>?>> RubricsForIssuesList(ProjectOwnedRequestModel req);

    /// <summary>
    /// Создать тему для обращений
    /// </summary>
    public Task<TResponseModel<int?>> RubricForIssuesCreateOrUpdate(RubricIssueHelpdeskModelDB issueTheme);

    /// <summary>
    /// Сдвинуть рубрику
    /// </summary>
    public Task<TResponseModel<bool?>> RubricForIssuesMove(RowMoveModel req);


    /// <summary>
    /// Получить обращения для пользователя
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>?>> IssuesSelect(GetIssuesForUserRequestModel req);

    /// <summary>
    /// Создать обращение
    /// </summary>
    public Task<TResponseModel<int>> IssueCreateOrUpdate(IssueHelpdeskModelDB issue);


    /// <summary>
    /// Сообщение из обращения помечается как ответ (либо этот признак снимается: в зависимости от запроса)
    /// </summary>
    public Task<TResponseModel<bool>> MessageOfIssueSetAsResponse(SetMessageAsResponseIssueRequestModel req);

    /// <summary>
    /// Добавить сообщение к обращению
    /// </summary>
    public Task<TResponseModel<int>> MessageForIssueCreateOrUpdate(IssueMessageHelpdeskBaseModel req);
}