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
}