////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Формы
/// </summary>
public class ConstructorFormBaseModel : EntryConstructedModel
{
    /// <summary>
    /// CSS класс формы
    /// </summary>
    public string? Css { get; set; } = "row";

    /// <summary>
    /// Текст кнопки: Добавить строку
    /// </summary>
    public string? AddRowButtonTitle { get; set; }
}