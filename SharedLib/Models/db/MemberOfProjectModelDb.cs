namespace SharedLib;

/// <summary>
/// Member of project
/// </summary>
public class MemberOfProjectModelDb : IdSwitchableModel
{
    /// <summary>
    ///  User (of Identity)
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public ProjectConstructorModelDb? Project {  get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public int ProjectId { get; set; }
}