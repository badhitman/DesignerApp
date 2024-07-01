////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Project (for constructor)
/// </summary>
[Index(nameof(SystemName)), Index(nameof(OwnerUserId))]
[Index(nameof(OwnerUserId), nameof(SystemName), IsUnique = true)]
public class ProjectConstructorModelDB : EntryDescriptionSwitchableModel
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
    public List<MemberOfProjectConstructorModelDB>? Members { get; set; }

    /// <summary>
    /// Scheme: Last updated DateTime
    /// </summary>
    public DateTime SchemeLastUpdated { get; set; } = DateTime.Now;


    /// <summary>
    /// Проверка прав/возможности пользователю редактировать данные в рамках проекта
    /// </summary>
    public bool CanEdit(UserInfoModel userInfoModel)
    {
        return !IsDisabled || OwnerUserId.Equals(userInfoModel.UserId) || userInfoModel.Roles?.Any(x => x.Equals("admin", StringComparison.OrdinalIgnoreCase)) == true;
    }

    /// <summary>
    /// Reload
    /// </summary>
    public void Reload(ProjectViewModel project)
    {
        Name = project.Name;
        SystemName = project.SystemName;
        Description = project.Description;
        Id = project.Id;
        IsDisabled = project.IsDisabled;
        OwnerUserId = project.OwnerUserId;
    }
}