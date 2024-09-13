////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Авторские данные
/// </summary>
[Index(nameof(UserPersonIdentityId))]
public class PersonalEntrySwitchableUpdatedModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Пользователь (Identity Id)
    /// </summary>
    public required string UserPersonIdentityId { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime LastAtUpdatedUTC { get; set; }
}