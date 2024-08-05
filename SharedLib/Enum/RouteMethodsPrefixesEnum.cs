////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Префиксы маршрутов контроллеров (+ Refit)
/// используется для генератора кода в контроллерах и службах refit
/// </summary>
public enum RouteMethodsPrefixesEnum
{
    /// <summary>
    /// Добавить/создать один объект
    /// </summary>
    [Description("Добавить/создать один объект")]
    AddSingle,

    /// <summary>
    /// Добавить/создать коллекцию объектов
    /// </summary>
    [Description("Добавить/создать коллекцию объектов")]
    AddRange,

    /// <summary>
    /// Получить один объект по идентификатору
    /// </summary>
    [Description("Получить один объект по идентификатору")]
    GetSingleById,

    /// <summary>
    /// Получить коллекцию элементов по идентификаторам
    /// </summary>
    [Description("Получить коллекцию элементов по идентификаторам")]
    GetRangeByIds,

    /// <summary>
    /// Получить коллекцию элементов (с пагинацией)
    /// </summary>
    [Description("Получить коллекцию элементов (с пагинацией)")]
    GetRangePagination,

    /// <summary>
    /// Получить коллекцию элементов по идентификатору ведущего объекта-владельца
    /// </summary>
    [Description("Получить коллекцию элементов по идентификатору ведущего объекта-владельца")]
    GetRangeByOwnerId,

    /// <summary>
    /// Обновить один объект
    /// </summary>
    [Description("Обновить один объект")]
    UpdateSingle,

    /// <summary>
    /// Обновить коллекцию объектов
    /// </summary>
    [Description("Обновить коллекцию объектов")]
    UpdateRange,

    /// <summary>
    /// Пометить объект по идентификатору
    /// </summary>
    [Description("Пометить объект по идентификатору")]
    MarkAsDeleteById,

    /// <summary>
    /// Удалить (безвозвратно) объект по идентификатору
    /// </summary>
    [Description("Удалить (безвозвратно) объект по идентификатору")]
    RemoveSingleById,

    /// <summary>
    /// Удалить (безвозвратно) коллекцию объектов по идентификаторам
    /// </summary>
    [Description("Удалить (безвозвратно) коллекцию объектов по идентификаторам")]
    RemoveRangeByIds
}