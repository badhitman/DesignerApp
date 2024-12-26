////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// WorkSchedulesViewModel
/// </summary>
public class WorkScheduleModel
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

    /// <summary>
    /// Слот уже начался (но не закончился)
    /// </summary>
    [Required]
    public required bool IsStarted { get; set; }


    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Date}: {StartPart}-{EndPart} [{Organization.Name}]";
    }

    /// <inheritdoc/>
    public static bool operator ==(WorkScheduleModel e1, WorkScheduleModel e2)
        => (e1 is null && e2 is null) || e1?.Equals(e2) == true;

    /// <inheritdoc/>
    public static bool operator !=(WorkScheduleModel e1, WorkScheduleModel e2)
        => !(e1 is null && e2 is null) && e1?.Equals(e2) != true;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is WorkScheduleModel other)
            return Date == other.Date && StartPart == other.StartPart && EndPart == other.EndPart && Organization.Id == other.Organization.Id;

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{Date}{StartPart}{EndPart}{Organization.Id}".GetHashCode();
    }
}