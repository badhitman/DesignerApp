////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Подбор организаций с параметризованным запросом-фильтром
/// </summary>
public class OrganizationsSelectRequestModel
{
    /// <summary>
    /// Идентификатор пользователя (Identity Id)
    /// </summary>
    public string? ForUserIdentityId { get; set; }

    /// <summary>
    /// Дата обновления должна быть не меньше указанной
    /// </summary>
    public DateTime? AfterDateUpdate { get; set; }

    /// <summary>
    /// Загрузить дополнительные данные (адреса) к объектам организаций
    /// </summary>
    public bool IncludeExternalData { get; set; }
}