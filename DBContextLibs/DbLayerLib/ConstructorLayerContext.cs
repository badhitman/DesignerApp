using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbLayerLib;

/// <summary>
/// LayerContext
/// </summary>
public partial class LayerContext
{
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
    public DbSet<LinkDirectoryToFormConstructorModelDB> LinksDirectoriesToForms { get; set; }


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
    public DbSet<TabJoinDocumentSchemeConstructorModelDB> TabsJoinsForms { get; set; }


    /// <summary>
    /// Сессии/ссылки для заполнения документов данными
    /// </summary>
    public DbSet<SessionOfDocumentDataModelDB> Sessions { get; set; }

    /// <summary>
    /// Значения/данные документов
    /// </summary>
    public DbSet<ValueDataForSessionOfDocumentModelDB> ValuesSessions { get; set; }
}