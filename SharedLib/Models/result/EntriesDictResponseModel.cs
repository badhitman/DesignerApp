namespace SharedLib;

/// <summary>
/// Entries (dict.entries) response
/// </summary>
public class EntriesDictResponseModel : ResponseBaseModel
{
    /// <summary>
    /// Entries (dict.entries)
    /// </summary>
    public IEnumerable<EntryDictModel>? Elements { get; set; }
}