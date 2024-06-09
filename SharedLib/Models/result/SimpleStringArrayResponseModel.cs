namespace SharedLib;

/// <summary>
/// Универсальная модель ответа коллекцией строк
/// </summary>
public class SimpleStringArrayResponseModel : ResponseBaseModel
{
    /// <summary>
    /// Данные ответа
    /// </summary>
    public IEnumerable<string>? Elements { get; set; }
}