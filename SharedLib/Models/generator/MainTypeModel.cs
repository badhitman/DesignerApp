////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib.Models;

/// <summary>
/// Вещественный тип (базовая модель для системных объектов)
/// </summary>
[Index(nameof(ProjectId), nameof(Name), IsUnique = true)]
[Index(nameof(ProjectId), nameof(SystemName), IsUnique = true)]
public abstract class MainTypeModel : RealTypeModel
{
    /// <summary>
    /// Внешний ключ на проект
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Проект, за которым закреплено перечисление
    /// </summary>
    public ProjectConstructorModelDb? Project { get; set; }

    /// <summary>
    /// Связи полей документов с типами данных
    /// </summary>
    public IEnumerable<DocumentPropertyLinkModelDB>? PropertiesTypesLinks { get; set; }
}
