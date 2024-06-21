namespace SharedLib;

/// <summary>
/// Формы
/// </summary>
public class ConstructorFormBaseModel : EntryDescriptionModel
{
    /// <summary>
    /// CSS класс формы
    /// </summary>
    public string? Css { get; set; } = "row";


    /// <summary>
    /// Текст кнопки: Добавить строку
    /// </summary>
    public string? AddRowButtonTitle { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public ProjectConstructorModelDb? Project { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public required int ProjectId { get; set; }
}