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

    /// <summary>
    /// System name
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.NAME_SPACE_TEMPLATE, ErrorMessage = GlobalStaticConstants.NAME_SPACE_TEMPLATE_MESSAGE)]
    public required string SystemName { get; set; }
}