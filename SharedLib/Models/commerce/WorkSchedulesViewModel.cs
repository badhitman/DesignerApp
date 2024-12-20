////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// WorkSchedulesViewModel
/// </summary>
public class WorkSchedulesViewModel
{
    /// <summary>
    /// Date
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Offer
    /// </summary>
    public OfferModelDB? Offer { get; set; }
    /// <summary>
    /// Offer
    /// </summary>
    public int? OfferId { get; set; }

    /// <summary>
    /// Ёмкость очереди (0 - безлимитное)
    /// </summary>
    public uint QueueCapacity { get; set; }
}