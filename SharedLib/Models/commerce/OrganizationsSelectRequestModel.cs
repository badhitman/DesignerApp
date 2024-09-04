////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// OrganizationsSelect
/// </summary>
public class OrganizationsSelectRequestModel
{
    /// <summary>
    /// For UserIdentity Id
    /// </summary>
    public string? ForUserIdentityId { get; set; }

    /// <summary>
    /// Дата обновления должна быть не меньше указанной
    /// </summary>
    public DateTime? AfterDateUpdate { get; set; }
}