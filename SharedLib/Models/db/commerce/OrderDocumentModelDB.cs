////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Заказ (документ)
/// </summary>
public class OrderDocumentModelDB : OrderDocumentBaseModelDB
{
    /// <summary>
    /// Адреса доставки
    /// </summary>
    public List<TabAddressForOrderModelDb>? AddressesTabs { get; set; }

    /// <summary>
    /// Подготовить объект заказа для записи в БД
    /// </summary>
    public void PrepareForSave()
    {
        Organization = null;
        AddressesTabs?.ForEach(x =>
        {
            x.AddressOrganization = null;
            x.Rows?.ForEach(y =>
            {
                y.Id = 0;
                y.Amount = y.Quantity * y.Offer!.Price;
                y.OrderDocument = this;
                y.Nomenclature = null;
                y.Offer = null;
                y.Version = Guid.NewGuid();
            });
        });
    }

    /// <summary>
    /// Сумма заказа всего
    /// </summary>
    public decimal TotalSumForRows()
    {
        if (AddressesTabs is null || AddressesTabs.Count == 0 || AddressesTabs.Any(x => x.Rows is null) || AddressesTabs.Any(x => x.Rows is null || x.Rows.Any(z => z.Offer is null)))
            return 0;

        return AddressesTabs.Sum(x => x.Rows!.Sum(y => y.Quantity * y.Offer!.Price));
    }
}