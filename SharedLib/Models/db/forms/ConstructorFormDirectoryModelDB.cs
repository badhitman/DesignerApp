using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Справоник/список
/// </summary>
[Index(nameof(Name), IsUnique = true)]
public class ConstructorFormDirectoryModelDB : EntryDescriptionModel
{
    /// <summary>
    /// Элементы справочника/списка
    /// </summary>
    public List<ConstructorFormDirectoryElementModelDB>? Elements { get; set; }

    /// <summary>
    /// Связи форм со списками/связями
    /// </summary>
    public List<ConstructorFormDirectoryLinkModelDB>? FormsDirectoriesLinks { get; set; }
}
