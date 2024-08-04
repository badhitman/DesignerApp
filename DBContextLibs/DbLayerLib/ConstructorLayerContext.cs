using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbLayerLib;

/// <summary>
/// LayerContext
/// </summary>
public partial class LayerContext
{
    /// <summary>
    /// Снапшоты, которые формируются при каждом удачном скачивании/генерации кода к проекту
    /// </summary>
    public DbSet<ProjectSnapshotModelDB> SnapshotsProjects { get; set; }

    /// <summary>
    /// Перечисления
    /// </summary>
    public DbSet<DirectoryEnumSnapshotModelDB> DirectoriesSnapshots { get; set; }

    /// <summary>
    /// Элементы перечислений
    /// </summary>
    public DbSet<DirectoryEnumElementSnapshotModelDB> ElementsOfDirectoriesSnapshots { get; set; }

    /// <summary>
    /// Документы
    /// </summary>
    public DbSet<DocumentSnapshotModelDB> DocumentsSnapshots { get; set; }

    /// <summary>
    /// Tabs snapshots
    /// </summary>
    public DbSet<TabSnapshotModelDB> TabsSnapshots { get; set; }

    /// <summary>
    /// Forms snapshots
    /// </summary>
    public DbSet<FormSnapshotModelDB> FormsSnapshots { get; set; }

    /// <summary>
    /// Directories fields documents snapshots
    /// </summary>
    public DbSet<FieldAkaDirectorySnapshotModelDB> FieldsAkaDirectoriesSnapshots { get; set; }

    /// <summary>
    /// Fields documents snapshots
    /// </summary>
    public DbSet<FieldSnapshotModelDB> FieldsSimplesSnapshots { get; set; }


    /// <summary>
    /// Projects
    /// </summary>
    public DbSet<ProjectConstructorModelDB> Projects { get; set; }

    /// <summary>
    /// Members of projects
    /// </summary>
    public DbSet<MemberOfProjectConstructorModelDB> MembersOfProjects { get; set; }

    /// <summary>
    /// Используемые проекты
    /// </summary>
    /// <remarks>Какой проект для какого пользователя установлен в роли основного/используемого</remarks>
    public DbSet<ProjectUseConstructorModelDb> ProjectsUse { get; set; }


    /// <summary>
    /// Справочники/списки
    /// </summary>
    public DbSet<DirectoryConstructorModelDB> Directories { get; set; }

    /// <summary>
    /// Элементы справочников/списков
    /// </summary>
    public DbSet<ElementOfDirectoryConstructorModelDB> ElementsOfDirectories { get; set; }


    /// <summary>
    /// Формы
    /// </summary>
    public DbSet<FormConstructorModelDB> Forms { get; set; }

    /// <summary>
    /// Поля форм
    /// </summary>
    public DbSet<FieldFormConstructorModelDB> Fields { get; set; }

    /// <summary>
    /// Связи форм со списками/справочниками
    /// </summary>
    public DbSet<FieldFormAkaDirectoryConstructorModelDB> LinksDirectoriesToForms { get; set; }


    /// <summary>
    /// Схемы документов
    /// </summary>
    public DbSet<DocumentSchemeConstructorModelDB> DocumentSchemes { get; set; }

    /// <summary>
    /// Табы/вкладки схем документов
    /// </summary>
    public DbSet<TabOfDocumentSchemeConstructorModelDB> TabsOfDocumentsSchemes { get; set; }

    /// <summary>
    /// Связи форм с табами/вкладками документов
    /// </summary>
    public DbSet<FormToTabJoinConstructorModelDB> TabsJoinsForms { get; set; }


    /// <summary>
    /// Сессии/ссылки для заполнения документов данными
    /// </summary>
    public DbSet<SessionOfDocumentDataModelDB> Sessions { get; set; }

    /// <summary>
    /// Значения/данные документов
    /// </summary>
    public DbSet<ValueDataForSessionOfDocumentModelDB> ValuesSessions { get; set; }
}