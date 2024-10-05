////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Стадии/шаги обращения: "Создан", "В работе", "На проверке" и "Готово"
/// </summary>
public enum HelpdeskIssueStepsEnum
{
    /// <summary>
    /// Создан
    /// </summary>
    [Description("Создан")]
    Created = 10,

    /// <summary>
    /// Возвращён в работу
    /// </summary>
    [Description("Возвращён")]
    Reopen = 20,

    /// <summary>
    /// Возвращён в работу
    /// </summary>
    [Description("Пауза")]
    Pause = 30,

    /// <summary>
    /// В работе
    /// </summary>
    [Description("В работе")]
    Progress = 40,

    /// <summary>
    /// На проверке
    /// </summary>
    [Description("Проверка")]
    Check = 50,

    /// <summary>
    /// Выполнен
    /// </summary>
    [Description("Выполнен")]
    Done = 60,

    /// <summary>
    /// Отменён
    /// </summary>
    [Description("Отменён")]
    Canceled = 70
}