////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////


namespace SharedLib;

/// <summary>
/// Заказ (документ)
/// </summary>
public class OrderDocumentModelDB : EntrySwitchableUpdatedModel
{
    /// <summary>
    /// IdentityUserId
    /// </summary>
    public required string AuthorIdentityUserId { get; set; }

    /// <summary>
    /// Идентификатор документа из внешней системы (например 1С)
    /// </summary>
    public string? ExternalDocumentId { get; set; }

    /// <summary>
    /// Дополнительная информация
    /// </summary>
    public string? Information { get; set; }

    /// <summary>
    /// Заявка, связанная с заказом.
    /// </summary>
    /// <remarks>
    /// До тех пор пока не указана заявка этот заказ всего лишь [Корзина]
    /// </remarks>
    public int? HelpdeskId { get; set; }

    /// <summary>
    /// Organization
    /// </summary>
    public OrganizationModelDB? Organization { get; set; }
    /// <summary>
    /// Organization
    /// </summary>
    public int OrganizationId { get; set; }

    /// <summary>
    /// AddressesTabs
    /// </summary>
    public List<TabAddressForOrderModelDb>? AddressesTabs { get; set; }

    /// <summary>
    /// Вложения (файлы)
    /// </summary>
    public List<AttachmentForOrderModelDB>? Attachments { get; set; }

    /// <summary>
    /// Подготовить объект заказа для записи в БД
    /// </summary>
    public void PrepareForSave()
    {
        Organization = null;
        AddressesTabs?.ForEach(x =>
        {
            //x.OrderDocument = null;
            x.AddressOrganization = null;

            x.Rows?.ForEach(y =>
            {
                y.Id = 0;
                y.OrderDocument = this;
                y.Goods = null;
                y.Offer = null;
            });
        });
    }

    /// <summary>
    /// Сумма заказа всего
    /// </summary>
    /// <returns></returns>
    public decimal TotalSumForRows()
    {
        if (AddressesTabs is null || AddressesTabs.Count == 0 || AddressesTabs.Any(x => x.Rows is null) || AddressesTabs.Any(x => x.Rows is null || x.Rows.Any(z => z.Offer is null)))
            return 0;

        return AddressesTabs.Sum(x => x.Rows!.Sum(y => y.Quantity * y.Offer!.Price));
    }
}