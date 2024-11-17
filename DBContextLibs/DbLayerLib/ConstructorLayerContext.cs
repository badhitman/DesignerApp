using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbcLib;

/// <summary>
/// LayerContext
/// </summary>
public partial class ConstructorLayerContext : DbContext
{
    /// <summary>
    /// Промежуточный/общий слой контекста базы данных
    /// </summary>
    public ConstructorLayerContext(DbContextOptions options)
        : base(options)
    {
        //#if DEBUG
        //        Database.EnsureCreated();
        //#else
        Database.Migrate();
        //#endif
    }

    /// <summary>
    /// Projects
    /// </summary>
    public DbSet<ManageManufactureModelDB> Manufactures { get; set; } = default!;       

    /// <summary>
    /// Системные имена
    /// </summary>
    public DbSet<ManufactureSystemNameModelDB> SystemNamesManufactures { get; set; } = default!;

    #region snapshots
    /// <summary>
    /// Снапшоты, которые формируются при каждом удачном скачивании/генерации кода к проекту
    /// </summary>
    public DbSet<ProjectSnapshotModelDB> ProjectsSnapshots { get; set; } = default!;

    /// <summary>
    /// Перечисления
    /// </summary>
    public DbSet<DirectoryEnumSnapshotModelDB> DirectoriesSnapshots { get; set; } = default!;

    /// <summary>
    /// Элементы перечислений
    /// </summary>
    public DbSet<DirectoryEnumElementSnapshotModelDB> ElementsOfDirectoriesSnapshots { get; set; } = default!;

    /// <summary>
    /// Документы
    /// </summary>
    public DbSet<DocumentSnapshotModelDB> DocumentsSnapshots { get; set; } = default!;

    /// <summary>
    /// Tabs snapshots
    /// </summary>
    public DbSet<TabSnapshotModelDB> TabsSnapshots { get; set; } = default!;

    /// <summary>
    /// Forms snapshots
    /// </summary>
    public DbSet<FormSnapshotModelDB> FormsSnapshots { get; set; } = default!;

    /// <summary>
    /// Fields simple snapshots
    /// </summary>
    public DbSet<FieldSnapshotModelDB> FieldsSimpleSnapshots { get; set; } = default!;

    /// <summary>
    /// Fields directory snapshots
    /// </summary>
    public DbSet<FieldAkaDirectorySnapshotModelDB> FieldsDirectorySnapshots { get; set; } = default!;

    /// <summary>
    /// FieldsSnapshots
    /// </summary>
    public DbSet<BaseFieldModel> FieldsSnapshots { get; set; } = default!;
    #endregion

    #region projects
    /// <summary>
    /// Projects
    /// </summary>
    public DbSet<ProjectModelDb> Projects { get; set; } = default!;

    /// <summary>
    /// Members of projects
    /// </summary>
    public DbSet<MemberOfProjectConstructorModelDB> MembersOfProjects { get; set; } = default!;

    /// <summary>
    /// Используемые проекты
    /// </summary>
    /// <remarks>Какой проект для какого пользователя установлен в роли основного/используемого</remarks>
    public DbSet<ProjectUseConstructorModelDb> ProjectsUse { get; set; } = default!;
    #endregion

    #region directories
    /// <summary>
    /// Справочники/списки
    /// </summary>
    public DbSet<DirectoryConstructorModelDB> Directories { get; set; } = default!;     

    /// <summary>
    /// Элементы справочников/списков
    /// </summary>
    public DbSet<ElementOfDirectoryConstructorModelDB> ElementsOfDirectories { get; set; } = default!;
    #endregion

    #region forms
    /// <summary>
    /// Формы
    /// </summary>
    public DbSet<FormConstructorModelDB> Forms { get; set; } = default!;

    /// <summary>
    /// Поля форм
    /// </summary>
    public DbSet<FieldFormConstructorModelDB> Fields { get; set; } = default!;

    /// <summary>
    /// Связи форм со списками/справочниками
    /// </summary>
    public DbSet<FieldFormAkaDirectoryConstructorModelDB> LinksDirectoriesToForms { get; set; } = default!;
    #endregion

    #region schemes
    /// <summary>
    /// Схемы документов
    /// </summary>
    public DbSet<DocumentSchemeConstructorModelDB> DocumentSchemes { get; set; } = default!;

    /// <summary>
    /// Табы/вкладки схем документов
    /// </summary>
    public DbSet<TabOfDocumentSchemeConstructorModelDB> TabsOfDocumentsSchemes { get; set; } = default!;

    /// <summary>
    /// Связи форм с табами/вкладками документов
    /// </summary>
    public DbSet<FormToTabJoinConstructorModelDB> TabsJoinsForms { get; set; } = default!;
    #endregion

    #region outer-joins
    /// <summary>
    /// Внешние связи форм с другими проектами
    /// </summary>
    public DbSet<FormOuterLinkModelDB> FormsOuterJoins { get; set; } = default!;

    /// <summary>
    /// Внешние связи перечислений с другими проектами
    /// </summary>
    public DbSet<DirectoryOuterLinkModelDB> DirectoriesOuterJoins { get; set; } = default!;

    /// <summary>
    /// Внешние связи документов с другими проектами
    /// </summary>
    public DbSet<DocumentOuterLinkModelDB> DocumentsOuterJoins { get; set; } = default!;
    #endregion

    #region public sessions
    /// <summary>
    /// Сессии/ссылки для заполнения документов данными
    /// </summary>
    public DbSet<SessionOfDocumentDataModelDB> Sessions { get; set; } = default!;

    /// <summary>
    /// Значения/данные документов
    /// </summary>
    public DbSet<ValueDataForSessionOfDocumentModelDB> ValuesSessions { get; set; } = default!;
    #endregion
}