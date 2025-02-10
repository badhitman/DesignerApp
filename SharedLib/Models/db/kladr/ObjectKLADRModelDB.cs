////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <inheritdoc/>
[Index(nameof(STATUS))]
public class ObjectKLADRModelDB : StreetKLADRModelDB
{
    /// <inheritdoc/>
    [Required, StringLength(1)]
    public required string STATUS { get; set; }
}