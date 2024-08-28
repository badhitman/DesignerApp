////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace SharedLib;

/// <summary>
/// Поле формы: Справочник/Список/Перечисление
/// </summary>
public class FieldAkaDirectoryFitModel : BaseRequiredFormFitModel
{
    /// <summary>
    /// Системное Имя : Справочник/Список/Перечисление
    /// </summary>
    public required string DirectorySystemName { get; set; }

    /// <summary>
    /// Элементы перечисления
    /// </summary>
    public required IEnumerable<EntryModel> Items { get; set; }

    /// <summary>
    /// Множественный выбор
    /// </summary>
    public required bool IsMultiSelect { get; set; }
}