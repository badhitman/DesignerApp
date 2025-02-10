////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// BaseKladrModel
/// </summary>
[Index(nameof(NAME)), Index(nameof(CODE))]
public class BaseKladrModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <inheritdoc/>
    [Required, StringLength(40)]
    public required string NAME { get; set; }

    /// <inheritdoc/>
    [Required, StringLength(17)]
    public required string CODE { get; set; }
}