////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Перечисления (enum)
/// </summary>
public class EnumDesignModelDB : RealTypeModel
{
    /// <summary>
    /// Состав/элементы перечисления
    /// </summary>
    public ICollection<EnumDesignItemModelDB>? EnumItems { get; set; }

    /// <summary>
    /// Внешний ключ на проект
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Проект, за которым закреплено перечисление
    /// </summary>
    public ProjectConstructorModelDB? Project { get; set; }

    /// <summary>
    /// Связи полей документов с типами данных
    /// </summary>
    public IEnumerable<DocumentPropertyLinkModelDB>? PropertiesTypesLinks { get; set; }
}
