////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Доставка
/// </summary>
public class DeliveryModelDb
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// OrderDocument
    /// </summary>
    public OrderDocumentModelDB? OrderDocument { get; set; }

    /// <summary>
    /// OrderDocument
    /// </summary>
    public int OrderDocumentId { get; set; }


    /// <summary>
    /// AddressOrganization
    /// </summary>
    public AddressOrganizationModelDB? AddressOrganization { get; set; }

    /// <summary>
    /// AddressOrganization
    /// </summary>
    public int AddressOrganizationId { get; set; }

    /// <summary>
    /// Цена доставки
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Статус доставки
    /// </summary>
    public HelpdeskIssueStepsEnum Status { get; set; }
}