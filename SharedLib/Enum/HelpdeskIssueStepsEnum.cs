﻿////////////////////////////////////////////////
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
    [Description("Возвращён в работу")]
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
    [Description("На проверке")]
    Check = 50,

    /// <summary>
    /// Готово
    /// </summary>
    [Description("Готово")]
    Done = 60,

    /// <summary>
    /// Отменён
    /// </summary>
    [Description("Отменён")]
    Canceled = 70
}