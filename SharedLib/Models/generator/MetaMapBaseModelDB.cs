////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib.Models;

/// <summary>
/// Поле/свойство объекта (базовая модель)
/// </summary>
[Index(nameof(PropertyType))]
[Index(nameof(PropertyLinkId))]
public abstract class MetaMapBaseModelDB : RealTypeModel
{
    /// <summary>
    /// Тип поля (перечисление, документ)
    /// </summary>
    public PropertyTypesEnum PropertyType { get; set; }

    /// <summary>
    /// Идентификатор ссылки на связанный тип
    /// </summary>
    public int? PropertyLinkId { get; set; }
    /// <summary>
    /// Связанный тип
    /// </summary>
    public DocumentPropertyLinkModelDB? PropertyLink { get; set; }

    /// <summary>
    /// Порядок/сортировка полей
    /// </summary>
    public uint SortIndex { get; set; }
}