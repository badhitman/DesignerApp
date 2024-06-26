////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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