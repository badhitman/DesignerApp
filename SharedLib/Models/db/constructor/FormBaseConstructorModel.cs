////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Формы
/// </summary>
public class FormBaseConstructorModel : EntryConstructedModel
{
    /// <summary>
    /// CSS класс формы
    /// </summary>
    /// <remarks>default: css="row"</remarks>
    public string? Css { get; set; } = "row";

    /// <summary>
    /// Текст кнопки: Добавить строку
    /// </summary>
    public string? AddRowButtonTitle { get; set; }
}