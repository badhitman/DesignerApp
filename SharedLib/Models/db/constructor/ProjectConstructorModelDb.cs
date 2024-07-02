////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Project (for constructor)
/// </summary>
[Index(nameof(OwnerUserId))]
public class ProjectConstructorModelDB : EntryDescriptionSwitchableModel
{
    /// <summary>
    ///  Owner user (of Identity)
    /// </summary>
    public required string OwnerUserId { get; set; }

    /// <summary>
    /// Members
    /// </summary>
    public List<MemberOfProjectConstructorModelDB>? Members { get; set; }

    /// <summary>
    /// Справочники/перечисления
    /// </summary>
    public List<DirectoryConstructorModelDB>? Directories { get; set; }

    /// <summary>
    /// Формы
    /// </summary>
    public List<FormConstructorModelDB>? Forms { get; set; }

    /// <summary>
    /// Схемы документов
    /// </summary>
    public List<DocumentSchemeConstructorModelDB>? Documents { get; set; }


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
        Description = project.Description;
        Id = project.Id;
        IsDisabled = project.IsDisabled;
        OwnerUserId = project.OwnerUserId;
    }
}