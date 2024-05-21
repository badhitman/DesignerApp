namespace SharedLib;

/// <summary>
/// Entries
/// </summary>
public class EntriesPaginationModel : PaginationResponseModel
{
    /// <summary>
    /// Entries
    /// </summary>
    public required List<EntryAltModel> Entries { get; set; }
}