////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// OrganizationContractorModel
/// </summary>
public class OrganizationContractorModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Организация
    /// </summary>
    public OrganizationModelDB? Organization { get; set; }

    /// <summary>
    /// Организация
    /// </summary>
    public int OrganizationId { get; set; }

    /// <summary>
    /// Offer
    /// </summary>
    public OfferModelDB? Offer { get; set; }
    /// <summary>
    /// Offer
    /// </summary>
    public int? OfferId { get; set; }
}