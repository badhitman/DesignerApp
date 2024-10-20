////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Project (for context)
/// </summary>
[Index(nameof(OwnerUserId)), Index(nameof(NormalizedUpperName)), Index(nameof(ContextName))]
public class ProjectModelDb : EntryDescriptionSwitchableModel
{
    /// <summary>
    /// NormalizedUpperName
    /// </summary>
    public required string NormalizedUpperName { get; set; }

    /// <summary>
    /// Имя контекста. по умолчанию: null (null or empty or spice) - используется для конструктора
    /// </summary>
    public string? ContextName { get; set; }

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
    /// SnapShots
    /// </summary>
    public List<ProjectSnapshotModelDB>? SnapShots { get; set; }


    /// <summary>
    /// Scheme: Last updated DateTime
    /// </summary>
    public DateTime SchemeLastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Проверка прав/возможности пользователю редактировать данные в рамках проекта
    /// </summary>
    public bool CanEdit(UserInfoModel userInfoModel)
    {
        return !IsDisabled || OwnerUserId.Equals(userInfoModel.UserId) || userInfoModel.Roles?.Any(x => x.Equals(GlobalStaticConstants.Roles.Admin, StringComparison.OrdinalIgnoreCase)) == true;
    }
}