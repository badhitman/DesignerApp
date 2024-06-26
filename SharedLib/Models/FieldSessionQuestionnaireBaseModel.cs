﻿namespace SharedLib;

/// <summary>
/// Строки данных таблиц
/// </summary>
public class FieldSessionQuestionnaireBaseModel
{
    /// <summary>
    /// Сессия опроса/анкеты
    /// </summary>
    public int SessionId { get; set; }

    /// <summary>
    /// Связь формы со страницей опроса
    /// </summary>
    public int JoinFormId { get; set; }
}