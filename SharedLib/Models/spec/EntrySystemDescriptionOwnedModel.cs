////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// EntryDescriptionOwnedModel
/// </summary>
public class EntrySystemDescriptionOwnedModel : EntryDescriptionModel
{
    /// <summary>
    /// Форма
    /// </summary>
    public int OwnerId { get; set; }
}