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
    public Task<TResponseModel<RubricIssueHelpdeskModelDB[]?>> GetRubricsIssues(ProjectOwnedRequestModel req);

    /// <summary>
    /// Создать тему для обращений
    /// </summary>
    public Task<TResponseModel<int>> CreateOrUpdateRubricIssues(RubricIssueHelpdeskModelDB issueTheme);


    /// <summary>
    /// Получить обращения для пользователя
    /// </summary>
    public Task<TResponseModel<IssueHelpdeskModelDB[]?>> GetIssuesForUser(GetIssuesForUserRequestModel req);

    /// <summary>
    /// Создать обращение
    /// </summary>
    public Task<TResponseModel<int>> CreateOrUpdateIssue(IssueHelpdeskModelDB issue);


    /// <summary>
    /// Сообщение из обращения помечается как ответ (либо этот признак снимается: в зависимости от запроса)
    /// </summary>
    public Task<TResponseModel<bool>> SetMessageIssueAsResponse(SetMessageAsResponseIssueRequestModel req);

    /// <summary>
    /// Добавить сообщение к обращению
    /// </summary>
    public Task<TResponseModel<int>> AddNewMessageIntoIssue(IssueMessageHelpdeskBaseModel req);

    /// <summary>
    /// UpdateMessageIssue
    /// </summary>
    public Task<TResponseModel<bool>> UpdateMessageIssue(UpdateMessageRequestModel req);
}