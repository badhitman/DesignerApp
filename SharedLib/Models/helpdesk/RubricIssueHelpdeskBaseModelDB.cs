﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Рубрики для обращений
/// </summary>
public class RubricIssueHelpdeskBaseModelDB : RubricIssueHelpdeskLowModel
{
    /// <summary>
    /// Имя контекста для разделения различных селекторов независимо друг от друга
    /// </summary>
    /// <remarks>
    /// Рубрики Helpdesk имеют значение контекста NULL. А подсистема адресов (Регионы/Города) используют этот эту же службу с указанием на имя контекста: <see cref="GlobalStaticConstants.Routes.ADDRESS_CONTROLLER_NAME"/> 
    /// </remarks>
    public string? ContextName { get; set; }

    /// <summary>
    /// ToUpper
    /// </summary>
    public string? NormalizedNameUpper { get; set; }

    /// <inheritdoc/>
    public List<RubricIssueHelpdeskModelDB>? NestedRubrics { get; set; }
}