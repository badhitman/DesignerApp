////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Universal update - request
/// </summary>
public class UniversalUpdateRequestModel : EntryDescriptionModel
{
    /// <summary>
    /// Parent ID
    /// </summary>
    public required int? ParentId { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    public int ProjectId { get; set; }
}