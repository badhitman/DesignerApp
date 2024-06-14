﻿using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Member of project
/// </summary>
public class MemberOfProjectModelDb
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
    public ProjectConstructorModelDb? Project {  get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public int ProjectId { get; set; }
}