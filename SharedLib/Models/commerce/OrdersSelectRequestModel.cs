////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// OrdersSelectRequestModel
/// </summary>
public class OrdersSelectRequestModel
{
    /// <summary>
    /// Дата обновления должна быть не меньше указанной
    /// </summary>
    public DateTime? AfterDateUpdate { get; set; }

    /// <summary>
    /// Только корзины
    /// </summary>
    /// <remarks>
    /// Заказы, у которых не указан связанная заявка в HelpDesk считается не оформленным заказом, а предварительным (корзина)
    /// </remarks>
    public bool CartOnly { get; set; }
}