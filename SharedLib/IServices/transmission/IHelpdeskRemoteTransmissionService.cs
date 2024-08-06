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
    public Task<TResponseModel<IssueThemeModelDB[]?>> GetThemesIssues();

    /// <summary>
    /// Создать тему для обращений
    /// </summary>
    public Task<TResponseModel<int>> CreateIssuesTheme(IssueThemeModelDB issueTheme);

    /// <summary>
    /// Получить обращения для пользователя
    /// </summary>
    public Task<TResponseModel<IssueModelDB[]?>> GetIssuesForUser(TPaginationRequestModel<(long? telegramId, string? identityId)> user);

    /// <summary>
    /// Создать обращение
    /// </summary>
    public Task<TResponseModel<int>> CreateIssue(IssueModelDB issue);
}