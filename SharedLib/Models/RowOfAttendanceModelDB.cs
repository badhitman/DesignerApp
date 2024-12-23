////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// RowOfAttendanceModelDB
/// </summary>
public class RowOfAttendanceModelDB : RowOfBaseDocumentModel
{
    /// <summary>
    /// DateAttendance
    /// </summary>
    public required DateOnly DateAttendance { get; set; }

    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public OrderAttendanceModelDB? OrderDocument { get; set; }

    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public int? OrderDocumentId { get; set; }

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
    /// Version
    /// </summary>
    [ConcurrencyCheck]
    public Guid Version { get; set; }
}