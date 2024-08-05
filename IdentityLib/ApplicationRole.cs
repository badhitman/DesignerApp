////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityLib;

/// <inheritdoc/>
public class ApplicationRole : IdentityRole
{
    /// <inheritdoc/>
    public ApplicationRole() : base()
    {
    }

    /// <inheritdoc/>
    public ApplicationRole(string roleName) : base(roleName)
    {
    }
    /// <summary>
    /// Роль
    /// </summary>
    [Display(Name = "Роль")]
    public override string? Name { get; set; }

    /// <summary>
    /// Заголовок
    /// </summary>
    [Display(Name = "Заголовок")]
    public string? Title { get; set; }
}