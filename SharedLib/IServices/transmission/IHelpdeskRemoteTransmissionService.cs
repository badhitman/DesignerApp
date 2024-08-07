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
    public Task<TResponseModel<IssueThemeHelpdeskModelDB[]?>> GetThemesIssues();

    /// <summary>
    /// Создать тему для обращений
    /// </summary>
    public Task<TResponseModel<int>> CreateIssuesTheme(IssueThemeHelpdeskModelDB issueTheme);

    /// <summary>
    /// Получить обращения для пользователя
    /// </summary>
    public Task<TResponseModel<IssueHelpdeskModelDB[]?>> GetIssuesForUser(TPaginationRequestModel<UserCrossIdsModel> user);

    /// <summary>
    /// Создать обращение
    /// </summary>
    public Task<TResponseModel<int>> CreateIssue(IssueHelpdeskModelDB issue);
}