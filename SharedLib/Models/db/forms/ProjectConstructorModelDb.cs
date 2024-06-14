using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Project (for constructor)
/// </summary>
[Index(nameof(OwnerUserId))]
public class ProjectConstructorModelDb : EntryDescriptionModel
{
    /// <summary>
    /// System name
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.NAME_SPACE_TEMPLATE, ErrorMessage = "Системное имя не корректное. Оно должно начинаться с буквы. Может содержать: буквы и цифры")]
    public required string SystemName { get; set; }

    /// <summary>
    ///  Owner user (of Identity)
    /// </summary>
    public required string OwnerUserId { get; set; }

    /// <summary>
    /// Members
    /// </summary>
    public List<MemberOfProjectModelDb>? Members { get; set; }
}