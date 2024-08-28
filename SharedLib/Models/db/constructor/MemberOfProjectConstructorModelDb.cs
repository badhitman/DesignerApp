////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Member of project
/// </summary>
[Index(nameof(UserId))]
public class MemberOfProjectConstructorModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    ///  User (of Identity)
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public ProjectConstructorModelDB? Project {  get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public int ProjectId { get; set; }
}