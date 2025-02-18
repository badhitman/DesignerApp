////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ApiRestBaseModel
/// </summary>
public abstract class ApiRestBaseModel : EntryModel
{

    /// <summary>
    /// Родитель
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// Родитель
    /// </summary>
    public ApiRestConfigModelDB? Parent { get; set; }
}