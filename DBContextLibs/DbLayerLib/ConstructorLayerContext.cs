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
    public DbSet<ProjectConstructorModelDb> Projects { get; set; }

    /// <summary>
    /// Members of projects
    /// </summary>
    public DbSet<MemberOfProjectModelDb> MembersOfProjects { get; set; }

    /// <summary>
    /// Enumerations
    /// </summary>
    public DbSet<EnumEntryModelDb> Enumerations { get; set; }

    /// <summary>
    /// Elements of enumerations
    /// </summary>
    public DbSet<ElementOfEnumEntryModelDb> ElementsOfEnumerations { get; set; }



    /// <summary>
    /// Формы
    /// </summary>
    public DbSet<ConstructorFormModelDB> Forms { get; set; }

    /// <summary>
    /// Поля форм
    /// </summary>
    public DbSet<ConstructorFieldFormModelDB> Fields { get; set; }

    /// <summary>
    /// Связи форм со списками/справочниками
    /// </summary>
    public DbSet<ConstructorFormDirectoryLinkModelDB> FormsDirectoriesLinks { get; set; }

    /// <summary>
    /// Справочники/списки
    /// </summary>
    public DbSet<ConstructorFormDirectoryModelDB> Directories { get; set; }

    /// <summary>
    /// Элементы справочников/списков
    /// </summary>
    public DbSet<ConstructorFormDirectoryElementModelDB> DirectoriesElements { get; set; }



    /// <summary>
    /// Сессии опросов/анкет
    /// </summary>
    public DbSet<ConstructorFormSessionModelDB> Sessions { get; set; }

    /// <summary>
    /// Значения опросов/анкет
    /// </summary>
    public DbSet<ConstructorFormSessionValueModelDB> ValuesSessions { get; set; }



    /// <summary>
    /// Опросы/анкеты
    /// </summary>
    public DbSet<ConstructorFormQuestionnaireModelDB> Questionnaires { get; set; }

    /// <summary>
    /// Страницы опросов/анкет
    /// </summary>
    public DbSet<ConstructorFormQuestionnairePageModelDB> QuestionnairesPages { get; set; }

    /// <summary>
    /// Связи форм со страницами опросов/анкет
    /// </summary>
    public DbSet<ConstructorFormQuestionnairePageJoinFormModelDB> QuestionnairesPagesJoinForms { get; set; }

}