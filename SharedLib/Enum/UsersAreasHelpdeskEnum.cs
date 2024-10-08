﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Users areas Helpdesk: Автор, Исполнитель, Подписчик или Исполнитель+Подписчик
/// </summary>
public enum UsersAreasHelpdeskEnum
{
    /// <summary>
    /// Автор
    /// </summary>
    [Description("Автор")]
    Author = 10,

    /// <summary>
    /// Исполнитель
    /// </summary>
    [Description("Исполнитель")]
    Executor = 20,

    /// <summary>
    /// Подписчик
    /// </summary>
    [Description("Подписчик")]
    Subscriber = 30,

    /// <summary>
    /// Executor || Subscriber
    /// </summary>
    [Description("Исполнитель или Подписчик")]
    Main = 50
}