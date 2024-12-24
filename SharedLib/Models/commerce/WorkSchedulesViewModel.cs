////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// WorkSchedulesViewModel
/// </summary>
public class WorkSchedulesViewModel
{
    /// <summary>
    /// Организация
    /// </summary>
    [Required]
    public required OrganizationModelDB Organization { get; set; }

    /// <summary>
    /// Date
    /// </summary>
    [Required]
    public required DateOnly Date { get; set; }

    /// <summary>
    /// StartPart
    /// </summary>
    [Required]
    public required TimeSpan StartPart { get; set; }

    /// <summary>
    /// EndPart
    /// </summary>
    [Required]
    public required TimeSpan EndPart { get; set; }

    /// <summary>
    /// Ёмкость очереди (0 - безлимитное)
    /// </summary>
    [Required]
    public required uint QueueCapacity { get; set; }

    /// <summary>
    /// IsGlobalPermission
    /// </summary>
    [Required]
    public required bool IsGlobalPermission { get; set; }
}