////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Базовая модель с поддержкой -> int:Id +string:Name +string:SystemName +bool:IsDeleted
/// </summary>
public class SystemEntryModel : EntryModel
{
    /// <summary>
    /// System name
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.NAME_SPACE_TEMPLATE, ErrorMessage = "Системное имя не корректное. Оно может содержать латинские буквы и цифры. Первым символом должна идти буква")]
    public required string SystemName { get; set; }
}