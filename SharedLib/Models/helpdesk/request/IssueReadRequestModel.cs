////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Получить данные обращения/инцидента
/// </summary>
public class IssueReadRequestModel
{
    /// <summary>
    /// Обращение/инцидент
    /// </summary>
    public required int IssueId {  get; set; }

    /// <summary>
    /// Без дополнительных данных. Если true - то <c>попутные</c> данные (рубрики и сообщения) не будут загружаться к ответу
    /// </summary>
    /// <remarks>
    /// Подписчики загружаются в любом случае
    /// </remarks>
    public bool IncludeSubscribersOnly { get; set; }
}