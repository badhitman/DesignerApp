////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Типы событий мониторинга инцидентов
/// </summary>
public enum PulseIssuesTypesEnum
{
    /// <summary>
    /// Изменение статуса
    /// </summary>
    /// <remarks>
    /// При создании обращения это первое событие для установки начального статуса: Создан
    /// </remarks>
    Status,

    /// <summary>
    /// Изменение в области темы, рубрики, описания
    /// </summary>
    Main,

    /// <summary>
    /// Изменения связанные с сообщениями
    /// </summary>
    Messages,

    /// <summary>
    /// Изменения исполнителя
    /// </summary>
    Executor,

    /// <summary>
    /// Голосование
    /// </summary>
    Vote,

    /// <summary>
    /// Изменения в подписчиках
    /// </summary>
    Subscribes,

    /// <summary>
    /// Изменения в файлах
    /// </summary>
    Files,
}