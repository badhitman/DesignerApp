////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <inheritdoc/>
public class AddressOrganizationModelDB : AddressOrganizationBaseModel
{
    /// <summary>
    /// Организация
    /// </summary>
    public OrganizationModelDB? Organization { get; set; }
}