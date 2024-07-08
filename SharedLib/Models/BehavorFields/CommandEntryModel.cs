////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Команда (в виде стандартного элемента)
/// </summary>
public class CommandEntryModel : EntryAltDescriptionModel
{
    /// <summary>
    /// Разрешение запуска/вызова без параметров
    /// </summary>
    public bool AllowCallWithoutParameters { get; set; } = false;
}