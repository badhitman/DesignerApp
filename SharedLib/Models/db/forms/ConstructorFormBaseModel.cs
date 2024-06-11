namespace SharedLib;

/// <summary>
/// Формы
/// </summary>
public class ConstructorFormBaseModel : SystemEntryModel
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