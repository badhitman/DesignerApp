////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ContractorsOrganizations
/// </summary>
public class ContractorsOrganizationsRequestModel
{
    /// <summary>
    /// OfferFilter
    /// </summary>
    public required int? OfferFilter { get; set; }

    /// <summary>
    /// Организации (фильтр)
    /// </summary>
    public int[]? OrganizationsFilter { get; set; }

    /// <summary>
    /// Загрузить дополнительные данные (адреса) к объектам организаций
    /// </summary>
    public bool IncludeExternalData { get; set; }
}