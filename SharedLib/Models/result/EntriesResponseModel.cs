namespace SharedLib;

/// <summary>
/// Элементы
/// </summary>
public class EntriesResponseModel : ResponseBaseModel
{
    /// <summary>
    /// Элементы
    /// </summary>
    public IEnumerable<EntryModel>? Entries { get; set; }
}