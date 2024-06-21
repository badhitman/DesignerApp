using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Project (for constructor)
/// </summary>
[Index(nameof(OwnerUserId))]
[Index(nameof(OwnerUserId), nameof(SystemName), IsUnique = true)]
public class ProjectConstructorModelDb : EntryDescriptionModel
{
    /// <summary>
    /// System name
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.NAME_SPACE_TEMPLATE, ErrorMessage = GlobalStaticConstants.NAME_SPACE_TEMPLATE_MESSAGE)]
    public required string SystemName { get; set; }

    /// <summary>
    ///  Owner user (of Identity)
    /// </summary>
    public required string OwnerUserId { get; set; }

    /// <summary>
    /// Members
    /// </summary>
    public List<MemberOfProjectModelDb>? Members { get; set; }

    /// <summary>
    /// Reload
    /// </summary>
    public void Reload(ProjectViewModel project)
    {

        Name = project.Name;
        SystemName = project.SystemName;
        Description = project.Description;

    }
}