////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// DeliveryForOrderUpdateRequestModel
/// </summary>
public class DeliveryForOrderUpdateRequestModel
{
    /// <summary>
    /// Delivery Address
    /// </summary>
    public int DeliveryAddressId { get; set; }

    /// <summary>
    /// Delivery Address
    /// </summary>
    public int OrderDocumentId { get; set; }

    /// <summary>
    /// Цена доставки
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Подключить доставку к заказу по указанному адресу доставки или отключить
    /// </summary>
    public bool SetAction { get; set; }

    /// <summary>
    /// Статус доставки
    /// </summary>
    public HelpdeskIssueStepsEnum Status { get; set; }
}