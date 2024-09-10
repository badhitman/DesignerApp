////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Адрес организации в заказе
/// </summary>
public class AddressForOrderModelDb
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
    /// Адрес доставки (если не самовывоз)
    /// </summary>
    /// <remarks>
    /// Если NULL - самовывоз
    /// </remarks>
    public int? DeliveryAddressId { get; set; }

    /// <summary>
    /// Стоимость доставки
    /// </summary>
    public decimal DeliveryPrice { get; set; }

    /// <summary>
    /// AddressOrganization
    /// </summary>
    public AddressOrganizationModelDB? AddressOrganization { get; set; }

    /// <summary>
    /// AddressOrganization
    /// </summary>
    public int AddressOrganizationId { get; set; }

    /// <summary>
    /// Строки заказа
    /// </summary>
    public List<RowOfOrderDocumentModelDB>? Rows { get; set; }
}