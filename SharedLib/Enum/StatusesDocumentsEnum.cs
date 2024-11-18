////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Стадии/шаги обращения: "Создан", "В работе", "На проверке" и "Готово"
/// </summary>
public enum StatusesDocumentsEnum
{
    /// <summary>
    /// Создан
    /// </summary>
    [Description("Создан")]
    Created = 0,

    /// <summary>
    /// Возвращён в работу
    /// </summary>
    [Description("Возвращён")]
    Reopen = 10,

    /// <summary>
    /// Возвращён в работу
    /// </summary>
    [Description("Пауза")]
    Pause = 20,

    /// <summary>
    /// В работе
    /// </summary>
    [Description("В работе")]
    Progress = 30,

    /// <summary>
    /// На проверке
    /// </summary>
    [Description("Проверка")]
    Check = 40,

    /// <summary>
    /// Выполнен
    /// </summary>
    [Description("Выполнен")]
    Done = 50,

    /// <summary>
    /// Отменён
    /// </summary>
    [Description("Отменён")]
    Canceled = 1000
}