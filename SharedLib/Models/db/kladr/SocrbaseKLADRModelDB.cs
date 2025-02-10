////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <inheritdoc/>
[Index(nameof(SCNAME)), Index(nameof(SOCRNAME)), Index(nameof(KOD_T_ST)), Index(nameof(LEVEL))]
public class SocrbaseKLADRModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <inheritdoc/>
    [Required, StringLength(5)]
    public required string LEVEL { get; set; }
    /// <inheritdoc/>
    [Required, StringLength(10)]
    public required string SCNAME { get; set; }
    /// <inheritdoc/>
    [Required, StringLength(29)]
    public required string SOCRNAME { get; set; }
    /// <inheritdoc/>
    [Required, StringLength(3)]
    public required string KOD_T_ST { get; set; }
}