////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Получить данные обращений/инцидентов
/// </summary>
public class IssuesReadRequestModel
{
    /// <summary>
    /// Обращения/инциденты
    /// </summary>
    public required int[] IssuesIds {  get; set; }

    /// <summary>
    /// Без дополнительных данных. Если true - то <c>попутные</c> данные (рубрики и сообщения) не будут загружаться к ответу
    /// </summary>
    /// <remarks>
    /// Подписчики загружаются в любом случае
    /// </remarks>
    public bool IncludeSubscribersOnly { get; set; }
}